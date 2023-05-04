using Sinerlog.Lambda.Pdf.Common.Constants;

namespace Sinerlog.Lambda.Pdf.Common.Messages
{
    public class CreateLabelMessage
    {
        public LabelTypeEnum LabelType { get; set; }
        public object LabelDtoBody { get; set; }
    }
}
