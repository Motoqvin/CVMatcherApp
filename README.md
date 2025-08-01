# 🧠 AI-Powered CV Analyzer & Job Matching API

An ASP.NET Core backend service that allows users to upload CVs (PDF/DOCX), analyze them using AI (OpenAI GPT), and evaluate match scores against job descriptions. Supports background job processing, analytics, JWT authentication, and health monitoring.

---

## 🚀 Features

- ✅ CV Upload (PDF, DOCX) with text extraction
- 🤖 AI-based CV analysis and skill extraction
- 📊 Job matching score and explanation via AI
- ⚙️ Asynchronous processing with status tracking
- 🔒 JWT-based authentication and audit logging
- 📈 Analytics per user and system-wide
- ❤️ Health checks & performance monitoring
- 🧪 Swagger documentation
- 🔁 Optional: Prometheus, Grafana, Application Insights

---

## 🛠️ Technologies

| Feature          | Stack                     |
|------------------|----------------------------|
| API Framework    | ASP.NET Core Web API       |
| AI Integration   | OpenAI GPT / ML.NET        |
| File Parsing     | `ITextSharp`, `OpenXml`, etc. |
| Jobs             | Hangfire                   |
| Auth             | JWT + ASP.NET Identity     |
| ORM              | Entity Framework Core      |
| Docs             | Swagger / OpenAPI          |
| Monitoring       | Health Checks, optional Prometheus/Grafana |
| Tests            | xUnit / Integration Tests  |

---

## 🔐 Authentication

- All endpoints require JWT authentication.
- Send the token in the `Authorization` header:

```http
Authorization: Bearer <your_token>
