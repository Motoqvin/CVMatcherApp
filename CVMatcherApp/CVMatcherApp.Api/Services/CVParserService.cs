using System.Text;
using System.Text.RegularExpressions;
using CVMatcherApp.Api.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Xceed.Words.NET;

namespace CVMatcherApp.Api.Services;

public class CVParserService
{
    public async Task<CV> ParseCVAsync(IFormFile file, string userId)
    {
        string content = await ExtractTextFromFileAsync(file);

        var cv = new CV
        {
            FileName = file.FileName,
            Content = content,
            FullName = ExtractFullName(content),
            Email = ExtractEmail(content),
            PhoneNumber = ExtractPhoneNumber(content),
            Location = ExtractLocation(content),
            Education = ExtractEducation(content),
            Experience = ExtractExperience(content),
            Skills = ExtractSkills(content),
            Summary = ExtractSummary(content),
            UserId = userId,
            IsParsed = true
        };

        return cv;
    }

    private async Task<string> ExtractTextFromFileAsync(IFormFile file)
    {
        using var ms = new MemoryStream();
        await file.CopyToAsync(ms);
        ms.Position = 0;

        if (file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
        {
            ms.Position = 0;
            using var reader = new PdfReader(ms);
            using var pdfDoc = new PdfDocument(reader);
            var strategy = new SimpleTextExtractionStrategy();
            var text = new StringBuilder();
            for (int i = 1; i <= pdfDoc.GetNumberOfPages(); i++)
            {
                var page = pdfDoc.GetPage(i);
                var pageText = iText.Kernel.Pdf.Canvas.Parser.PdfTextExtractor.GetTextFromPage(page, strategy);
                text.AppendLine(pageText);
            }
            return text.ToString();
        }

        if (file.FileName.EndsWith(".docx", StringComparison.OrdinalIgnoreCase))
        {
            StringBuilder textBuilder = new StringBuilder();

            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(ms, false))
        {
            MainDocumentPart mainPart = wordDoc.MainDocumentPart!;

            if (mainPart != null)
            {
                Body body = mainPart.Document.Body!;

                if (body != null)
                {
                    foreach (Paragraph p in body.Descendants<Paragraph>())
                    {
                            foreach (Run r in p.Descendants<Run>())
                            {
                                foreach (Text t in r.Descendants<Text>())
                                {
                                    textBuilder.Append($"{t.Text}");
                                }
                                textBuilder.Append(' ');
                        }
                        textBuilder.AppendLine();
                    }
                }
            }
        }
        return textBuilder.ToString();
        }

        throw new NotSupportedException("Unsupported file type");
    }

    private string? ExtractEmail(string text) =>
        Regex.Match(text, @"[a-zA-Z0-9\.\-_]+@[a-zA-Z0-9\-_]+\.[a-zA-Z]{2,}").Value;

    private string? ExtractPhoneNumber(string text) =>
        Regex.Match(text, @"\+?\d{1,3}[\s\-]?\(?\d{2,4}\)?[\s\-]?\d{3}[\s\-]?\d{3,4}").Value;

    private string? ExtractFullName(string text)
    {
        var lines = text.Split(' ');
        foreach (var line in lines.Take(10))
        {
            if (Regex.IsMatch(line.Trim(), @"^[A-Z][a-z]+(\s[A-Z][a-z]+)+$"))
                return line.Trim();
        }
        return null;
    }

    private string? ExtractLocation(string text)
    {
        var match = Regex.Match(text, @"(?i)(?:Address|Location)[:\s]+(.+)");
        return match.Success ? match.Groups[1].Value.Trim() : null;
    }

    private string? ExtractEducation(string text)
    {
        var educationKeywords = new[] { "Bachelor", "Master", "PhD", "University", "BSc", "MSc" };
        return ExtractSection(text, "Education", educationKeywords);
    }

    private string? ExtractExperience(string text)
    {
        var experienceKeywords = new[] { "Company", "Worked", "Experience", "Developer", "Engineer" };
        return ExtractSection(text, "Experience", experienceKeywords);
    }

    private string? ExtractSkills(string text)
    {
        var skillsKeywords = new[] { "C#", "Java", "SQL", "Python", "JavaScript", "ASP.NET", "HTML", "CSS" };
        var found = skillsKeywords.Where(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
        return string.Join(", ", found);
    }

    private string? ExtractSummary(string text)
    {
        var match = Regex.Match(text, @"(?i)(Summary|Objective)[:\s]+(.+?)(\n|\r|$)");
        return match.Success ? match.Groups[2].Value.Trim() : null;
    }

    private string? ExtractSection(string text, string sectionName, string[] keywords)
    {
        var lines = text.Split(' ');
        var index = Array.FindIndex(lines, l => l.Contains(sectionName, StringComparison.OrdinalIgnoreCase));
        if (index == -1) return null;

        var sectionLines = lines.Skip(index + 1).Take(10);
        return string.Join("\n", sectionLines.Where(l => keywords.Any(k => l.Contains(k, StringComparison.OrdinalIgnoreCase))));
    }
}