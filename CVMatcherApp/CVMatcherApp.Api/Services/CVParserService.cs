using System.Text;
using System.Text.RegularExpressions;
using CVMatcherApp.Api.Models;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml.Wordprocessing;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser.Listener;

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
            var textBuilder = new StringBuilder();
            using (WordprocessingDocument wordDoc = WordprocessingDocument.Open(ms, false))
            {
                var mainPart = wordDoc.MainDocumentPart!;
                if (mainPart != null)
                {
                    var body = mainPart.Document.Body!;
                    if (body != null)
                    {
                        foreach (var p in body.Descendants<Paragraph>())
                        {
                            foreach (var r in p.Descendants<Run>())
                            {
                                foreach (var t in r.Descendants<Text>())
                                {
                                    textBuilder.Append($"{t.Text}");
                                }
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
        var lines = text.Split('\n').Select(l => l.Trim()).Where(l => l.Length > 0).ToList();
        foreach (var line in lines.Take(8))
        {
            if (Regex.IsMatch(line, @"^[A-Z][a-z]+(\s[A-Z][a-z]+)+$") && !line.Contains("@") && !line.Contains("Engineer") && !line.Contains("Developer"))
                return line;
        }
        var match = Regex.Match(text, @"^([A-Z][a-z]+(\s[A-Z][a-z]+)+)\s*\n\s*Senior Software Engineer", RegexOptions.Multiline);
        if (match.Success)
            return match.Groups[1].Value.Trim();
        return null;
    }

    private string? ExtractLocation(string text)
    {
        var match = Regex.Match(text, @"Based in ([A-Za-z\s]+)", RegexOptions.IgnoreCase);
        if (match.Success) return match.Groups[1].Value.Trim();
        match = Regex.Match(text, @"Location[:\s]+([A-Za-z\s]+)", RegexOptions.IgnoreCase);
        if (match.Success) return match.Groups[1].Value.Trim();
        return null;
    }

    private string? ExtractEducation(string text) => ExtractSection(text, "Education");
    private string? ExtractExperience(string text) => ExtractSection(text, "Work Experience");

    private string? ExtractSkills(string text)
    {
        var section = ExtractSection(text, "Core Skills");
        if (!string.IsNullOrWhiteSpace(section))
            return section.Replace("\n", ", ").Replace("•", "").Replace(":", "").Trim(' ', ',');
        var skillsKeywords = new[] { "Angular", "C#", "Java", "JavaScript", "TypeScript", "SQL", "HTML", "CSS", "Docker", "REST", "RxJS", "JIRA", "Git", "Karma", "NGRX", "Jest", "Cypress", "TailwindCSS", "WinForms", "WPF", "SOAP", "SCSS", "Jasmine" };
        var found = skillsKeywords.Where(k => text.Contains(k, StringComparison.OrdinalIgnoreCase));
        return string.Join(", ", found.Distinct());
    }

    private string? ExtractSummary(string text)
    {
        var lines = text.Split('\n').Take(25).Select(l => l.Trim());
        foreach (var line in lines)
        {
            if (line.StartsWith("Senior Software Engineer") || line.StartsWith("Summary") || line.StartsWith("Profile"))
                return line;
        }
        return null;
    }
    
    private string? ExtractSection(string text, string sectionName)
    {
        var lines = text.Split('\n');
        var startIndex = Array.FindIndex(lines, l => l.Trim().Equals(sectionName, StringComparison.OrdinalIgnoreCase));
        if (startIndex == -1)
            return null;

        var sb = new StringBuilder();
        for (int i = startIndex + 1; i < lines.Length; i++)
        {
            var line = lines[i].Trim();

            if (IsSectionHeader(line)) break;
            if (!string.IsNullOrWhiteSpace(line))
                sb.AppendLine(line);
        }
        var result = sb.ToString().Trim();
        return string.IsNullOrWhiteSpace(result) ? null : result;
    }

    private bool IsSectionHeader(string line)
    {
        if (string.IsNullOrWhiteSpace(line)) return false;
        string[] sections = {
            "Projects", "Languages", "Awards", "Certificates", "Core Skills",
            "Work Experience", "Education", "Summary", "Profile", "Soft Skills", "Testing",
            "CI/CD & DevOps", "Backend", "Frontend", "Tools & Methodologies"
        };
        return sections.Any(s =>
            line.Equals(s, StringComparison.OrdinalIgnoreCase) ||
            line.StartsWith(s + ":", StringComparison.OrdinalIgnoreCase) ||
            line.StartsWith(s + " –", StringComparison.OrdinalIgnoreCase) ||
            line.StartsWith(s + " -", StringComparison.OrdinalIgnoreCase));
    }
}