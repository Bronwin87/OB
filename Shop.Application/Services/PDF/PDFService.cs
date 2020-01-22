using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using DinkToPdf;
using DinkToPdf.Contracts;
using Microsoft.AspNetCore.Http;

namespace Shop.Application.Services.PDF
{
    public class PDFService
    {
        private IConverter _converter;
        public PDFService(IConverter converter)
        {
            _converter = converter;
        }
        public async Task<FileDto> GetCartAsPDF(string html)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings()  {
                        HtmlContent = html
                    }
                }
            };
            return new FileDto("OfficeBox.pdf", _converter.Convert(doc));
        }

        public async Task<FileDto> GetProductAsPdf(string html)
        {
            var doc = new HtmlToPdfDocument()
            {
                GlobalSettings = {
                    PaperSize = PaperKind.A4,
                    Orientation = Orientation.Portrait
                },
                Objects = {
                    new ObjectSettings()  {
                       
                        HtmlContent = html
                    }
                }
            };
            return new FileDto("OfficeBoxProduct.pdf", _converter.Convert(doc));
        }
    }

    public class FileDto
    {
        public string FileName { get; set; }
        public byte[] FileBytes { get; set; }
        public FileDto(string fileName, byte[] fileBytes)
        {
            FileName = fileName;
            FileBytes = fileBytes;
        }
    }
}
