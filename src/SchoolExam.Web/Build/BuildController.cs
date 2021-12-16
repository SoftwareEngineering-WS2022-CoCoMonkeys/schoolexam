using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SchoolExam.Application.QrCode;
using SchoolExam.Core.UserManagement.UserAggregate;
using SchoolExam.Web.Build.Model;

namespace SchoolExam.Web.Build
{
    public class BuildController : ApiController<BuildController>
    {
        private readonly IQrCodeGenerator _qrCodeGenerator;
        private readonly IQrCodeDataGenerator _qrCodeDataGenerator;

        public BuildController(ILogger<BuildController> logger, IQrCodeGenerator qrCodeGenerator,
            IQrCodeDataGenerator qrCodeDataGenerator) : base(logger)
        {
            _qrCodeGenerator = qrCodeGenerator;
            _qrCodeDataGenerator = qrCodeDataGenerator;
        }

        [HttpPost]
        [Route("GenerateQrCodes")]
        [Authorize(Roles = $"{Roles.Administrator},{Roles.Teacher}")]
        public IEnumerable<GeneratedQrCodeModel> GenerateQrCodes([FromBody] GenerateQrCodesModel generateQrCodesModel)
        {
            var generatedQrCodes = Enumerable.Range(0, generateQrCodesModel.Count)
                .Select(i =>
                    _qrCodeGenerator.GeneratePngQrCode(
                        _qrCodeDataGenerator.Generate(string.Empty, DateTime.Now, Guid.Empty, 0), 1)).Select(x =>
                    new GeneratedQrCodeModel {RawData = x.qrCode, PngData = x.pngQrCode});

            return generatedQrCodes;
        }
    }
}