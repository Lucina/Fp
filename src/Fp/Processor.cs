using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;

namespace Fp;

/// <summary>
/// Base type for processors.
/// </summary>
[SuppressMessage("ReSharper", "UnusedMember.Global")]
[SuppressMessage("ReSharper", "MemberCanBePrivate.Global")]
[SuppressMessage("ReSharper", "UnusedMethodReturnValue.Global")]
[SuppressMessage("ReSharper", "NotAccessedField.Global")]
[SuppressMessage("ReSharper", "ClassNeverInstantiated.Global")]
[SuppressMessage("ReSharper", "ClassWithVirtualMembersNeverInherited.Global")]
public partial class Processor : IDisposable
{
    #region Properties and fields

    private const int StringDefaultCapacity = 4 * 1024;
    private const int StringExcessiveCapacity = 128 * 1024;

    /// <summary>
    /// Per-thread instance.
    /// </summary>
    public static Processor Instance => s_instance ??= new Processor { LogReceiver = NullLog.Instance };

    [ThreadStatic] private static Processor? s_instance;

    /// <summary>
    /// True if current platform allows backslashes as separators (modifies path creation / operation behaviour).
    /// </summary>
    public static readonly bool PlatformSupportBackSlash =
        Path.DirectorySeparatorChar == '\\' || Path.AltDirectorySeparatorChar == '\\';

    /// <summary>
    /// Whether to allow backslash as separator.
    /// </summary>
    public bool SupportBackSlash;

    /// <summary>
    /// Arguments.
    /// </summary>
    public IReadOnlyList<string> Args = null!;

    /// <summary>
    /// Log output target.
    /// </summary>
    public ILogReceiver LogReceiver = null!;

    /// <summary>
    /// Whether to preload newly opened file input streams to <see cref="MemoryStream"/>.
    /// </summary>
    public bool Preload;

    /// <summary>
    /// Input stream for current file if opened.
    /// </summary>
    public Stream? InputStream
    {
        get => _inputStream;
        set => SetInputStream(value);
    }

    private Stream? _inputStream;

    /// <summary>
    /// Input stream stack.
    /// </summary>
    private readonly Stack<Stream?> _inputStack = new(new[] { (Stream?)null });

    /// <summary>
    /// Length of input stream for current file if opened.
    /// </summary>
    public long InputLength => _inputStream?.Length ?? throw new InvalidOperationException();

    /// <summary>
    /// Output stream for current file if opened.
    /// </summary>
    public Stream? OutputStream { get; set; }

    /// <summary>
    /// Whether to read input as little-endian.
    /// </summary>
    public bool LittleEndian
    {
        get => _littleEndian;
        set
        {
            _littleEndian = value;
            _swap = BitConverter.IsLittleEndian ^ _littleEndian;
        }
    }

    private bool _littleEndian;

    private bool _swap;

    private MemoryStream TempMs => _tempMs ??= new MemoryStream();
    private MemoryStream? _tempMs;
    private static byte[] TempBuffer => s_tempBuffer ??= new byte[sizeof(long)];
    [ThreadStatic] private static byte[]? s_tempBuffer;

    private Encoder Utf8Encoder => _utf8Encoder ??= Encoding.UTF8.GetEncoder();
    private Encoder? _utf8Encoder;
    private Encoder?[] Utf16Encoders => _utf16Encoders ??= new Encoder?[GUtf16Encodings.Length];
    private Encoder?[]? _utf16Encoders;

    private Encoder GetUtf16Encoder(bool bigEndian, bool bom)
    {
        int i = (bigEndian ? 1 : 0) + (bom ? 2 : 0);
        return Utf16Encoders[i] ??= GUtf16Encodings[i].GetEncoder();
    }

    private static Encoding GetUtf16Encoding(bool bigEndian, bool bom) =>
        GUtf16Encodings[(bigEndian ? 1 : 0) + (bom ? 2 : 0)];

    private static Encoding[] GUtf16Encodings => s_gUtf16Encodings ??= new Encoding[] { new UnicodeEncoding(false, false), new UnicodeEncoding(true, false), new UnicodeEncoding(false, true), new UnicodeEncoding(true, true) };

    private static Encoding[]? s_gUtf16Encodings;

    /// <summary>
    /// Option keys.
    /// </summary>
    protected virtual string[] OptKeys => Array.Empty<string>();

    /// <summary>
    /// Input flags.
    /// </summary>
    public HashSet<string> Flags { get; set; } = null!;

    /// <summary>
    /// Input options.
    /// </summary>
    public Dictionary<string, string> Opts { get; set; } = null!;

    /// <summary>
    /// Input positional arguments.
    /// </summary>
    public List<string> PosArgs { get; set; } = null!;

    #endregion

    #region Constructor

    /// <summary>
    /// Creates a new instance of <see cref="Processor"/>.
    /// </summary>
    public Processor()
    {
        InitEncodingDecodingHelpers();
    }

    #endregion

    #region Main operation functions

    /// <summary>
    /// Prepare critical state for operation.
    /// </summary>
    /// <param name="configuration">Additional configuration object.</param>
    public void Prepare(ProcessorConfiguration? configuration = null)
    {
        Cleanup(true);
        LittleEndian = true;
        SupportBackSlash = false;
        if (configuration == null) return;
        Debug = configuration.Debug;
        Nop = configuration.Nop;
        Preload = configuration.Preload;
        LogReceiver = configuration.LogReceiver;
        Args = configuration.Args;
    }

    /// <summary>
    /// Setup arguments (flags, opts, posargs).
    /// </summary>
    public void SetupArgs()
    {
        (Flags, Opts, PosArgs) = Args.IsolateFlags(OptKeys);
    }

    #endregion

    #region Logging utilities

    /// <summary>
    /// Invoke logger with formatted string containing specified log.
    /// </summary>
    /// <param name="log">Message.</param>
    public void LogInfo(string log) => LogReceiver.LogInformation(log);

    /// <summary>
    /// Invoke logger with formatted string containing specified log.
    /// </summary>
    /// <param name="log">Message.</param>
    public void LogWarn(string log) => LogReceiver.LogWarning(log);

    /// <summary>
    /// Invoke logger with formatted string containing specified log.
    /// </summary>
    /// <param name="log">Message.</param>
    public void LogFail(string log) => LogReceiver.LogError(log);

    #endregion

    #region I/O stack

    /// <summary>
    /// Manages a processor's stream stack.
    /// </summary>
    private sealed class InputStackContext : IDisposable
    {
        /// <summary>
        /// Processor to operate on.
        /// </summary>
        private readonly Processor _processor;

        /// <summary>
        /// Stream to push.
        /// </summary>
        private readonly Stream _stream;

        private bool _disposed;

        /// <summary>
        /// Creates a new instance of <see cref="InputStackContext"/>.
        /// </summary>
        /// <param name="processor">Processor to operate on.</param>
        /// <param name="stream">Stream to push.</param>
        /// <remarks>
        /// This constructor also calls <see cref="PushInput"/>.
        /// </remarks>
        internal InputStackContext(Processor processor, Stream stream)
        {
            _processor = processor;
            _stream = stream;
            _processor.PushInput(_stream);
        }

        /// <inheritdoc />
        public void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            _stream.Dispose();
            _processor.PopInput();
        }
    }

    private void PushInput(Stream? stream)
    {
        _inputStack.Push(stream);
        _inputStream = stream;
    }

    private void PopInput()
    {
        if (_inputStack.Count == 1)
            throw new InvalidOperationException("Cannot pop input stack at root level");
        _inputStack.Pop();
        _inputStream = _inputStack.Peek();
    }

    private void SetInputStream(Stream? stream)
    {
        _inputStack.Pop();
        _inputStack.Push(stream);
        _inputStream = stream;
    }

    /// <summary>
    /// Creates a region of and overwrites current <see cref="InputStream"/>.
    /// </summary>
    /// <param name="offset">Offset relative to current stream.</param>
    /// <param name="length">Length of region.</param>
    /// <returns>Disposable context that will restore previous <see cref="InputStream"/> when disposed.</returns>
    /// <exception cref="InvalidOperationException">Thrown when <see cref="InputStream"/> is null or not seekable.</exception>
    public IDisposable Region(long offset, long? length = null)
    {
        if (_inputStream == null)
            throw new InvalidOperationException($"Cannot make region when {nameof(InputStream)} is null");
        if (!_inputStream.CanSeek)
            throw new InvalidOperationException($"Cannot make region when {nameof(InputStream)} is not seekable");

        var ins = _inputStream;
        // Go through sub-streams if possible to reduce chaining
        while (ins is SStream { CanSeek: true } sStr)
        {
            offset += sStr.Offset;
            ins = sStr.BaseStream;
        }

        ins.Position = offset;
        return new InputStackContext(this, new SStream(ins, length ?? ins.Length - offset));
    }

    #endregion

    #region Lifecycle

    /// <summary>
    /// Cleans up resources.
    /// </summary>
    /// <param name="warn">Warn if resources were not previously cleaned up.</param>
    public virtual void Cleanup(bool warn = false)
    {
        if (_inputStream != null)
        {
            _inputStream.Dispose();
            if (warn) LogReceiver.LogWarning("Input stream was not disposed prior to cleanup call");
            InputStream = null;
        }

        if (OutputStream != null)
        {
            OutputStream.Dispose();
            if (warn) LogReceiver.LogWarning("Output stream was not disposed prior to cleanup call");
            OutputStream = null;
        }

        MemClear();
    }

    #endregion

    #region Dispose pattern

    /// <summary>
    /// Disposes resources.
    /// </summary>
    /// <param name="disposing">False if called from finalizer.</param>
    // ReSharper disable once VirtualMemberNeverOverridden.Global
    // ReSharper disable once UnusedParameter.Global
    protected virtual void Dispose(bool disposing)
    {
    }

    /// <inheritdoc />
    public void Dispose()
    {
        Cleanup();
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #endregion
}