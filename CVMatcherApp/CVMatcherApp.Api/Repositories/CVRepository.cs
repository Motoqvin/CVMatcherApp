using CVMatcherApp.Api.Data;
using CVMatcherApp.Api.Exceptions;
using CVMatcherApp.Api.Models;
using CVMatcherApp.Api.Repositories.Base;
using DocumentFormat.OpenXml.Packaging;
using iText.Kernel.Pdf;
using iText.Kernel.Pdf.Canvas.Parser;
using iText.Kernel.Pdf.Canvas.Parser.Listener;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace CVMatcherApp.Api.Repositories;

public class CVRepository : ICVRepository
{
    private readonly MatcherDbContext dbContext;

    public CVRepository(MatcherDbContext dbContext)
    {
        this.dbContext = dbContext;
    }
    public async Task<int> DeleteOldCVsAsync()
    {
        var cutoff = DateTime.UtcNow.AddDays(-1);
        var oldCVs = await dbContext.CVs.Where(cv => cv.CreatedAt < cutoff).ToListAsync();

        dbContext.RemoveRange(oldCVs);
        await dbContext.SaveChangesAsync();

        return oldCVs.Count;
    }

    public CV ExtractPdf(byte[] bytes)
    {
        if (bytes == null || bytes.Length == 0)
            throw new ArgumentException("File cannot be null or empty");
        using var memoryStream = new MemoryStream(bytes);
        using var reader = new PdfReader(memoryStream);
        using var doc = new PdfDocument(reader);
        CV cv = new CV();
        if (doc.GetNumberOfPages() == 0)
            throw new BadRequestException("The PDF document is empty or invalid.", nameof(doc));

        for (int pageNum = 1; pageNum <= doc.GetNumberOfPages(); pageNum++)
        {
            ITextExtractionStrategy strategy = new SimpleTextExtractionStrategy();

            string pageText = PdfTextExtractor.GetTextFromPage(doc.GetPage(pageNum), strategy);
            cv.Content = pageText;

            Console.WriteLine($"--- Page {pageNum} ---");
            Console.WriteLine(pageText);
        }

        return cv;
    }

    public CV ExtractDocx(byte[] bytes)
    {
        using var stream = new MemoryStream(bytes);
        using WordprocessingDocument wordDoc = WordprocessingDocument.Open(stream, false);
        var body = wordDoc.MainDocumentPart?.Document.Body;
        var cv = new CV()
        {
            Content = body?.InnerText
        };

        System.Console.WriteLine(body?.InnerText);
        return cv;
    }

    public Task<List<CV>> GetAllCVsAsync(string userId)
    {
        return dbContext.CVs.Where(cv => cv.UserId == userId).ToListAsync();
    }

    public async Task<CV> GetCVByIdAsync(int id)
    {
        return await dbContext.CVs.FirstOrDefaultAsync(cv => cv.Id == id) ?? throw new Exception($"CV with ID {id} not found.");
    }

    public Task<int> SaveCVAsync(CV cv)
    {
        if (cv.Id == 0)
        {
            cv.CreatedAt = DateTime.UtcNow;
            dbContext.CVs.Add(cv);
        }
        else
        {
            dbContext.CVs.Update(cv);
        }

        return dbContext.SaveChangesAsync();
    }

    public Task<bool> UpdateCVAsync(CV cv)
    {
        dbContext.CVs.Update(cv);
        return dbContext.SaveChangesAsync().ContinueWith(t => t.Result > 0);
    }
}