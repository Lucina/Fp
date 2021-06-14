using System.Collections.Generic;

namespace Fp
{
    /// <summary>
    /// Represents a segmented processor that automatically opens <see cref="Processor.InputStream"/>.
    /// </summary>
    public class DataProcessor : Processor
    {
        /// <inheritdoc />
        protected sealed override void ProcessImpl()
        {
            base.ProcessImpl();
        }

        /// <inheritdoc />
        protected sealed override IEnumerable<Data> ProcessSegmentedImpl()
        {
            OpenMainFile();
            return ProcessData();
        }

        /// <summary>
        /// Process current file in parts
        /// </summary>
        /// <returns>Generated outputs</returns>
        protected virtual IEnumerable<Data> ProcessData() => Nothing;
    }
}
