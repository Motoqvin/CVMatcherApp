# AI-Powered CV Analyzer & Job Matching API (Backend Only)

## Overview

This is a backend-only RESTful API built with **ASP.NET Core**, integrating **AI technologies** for **CV parsing**, **job matching**, and **performance monitoring**. It supports asynchronous job execution, JWT-based security, and real-time analytics.

---

## Table of Contents

- [Features](#features)
- [Tech Stack](#tech-stack)
- [API Endpoints](#api-endpoints)
  - [CV Upload](#cv-upload)
  - [Start CV Analysis](#start-cv-analysis)
  - [Get Analysis Result](#get-analysis-result)
  - [Analytics & Monitoring](#analytics--monitoring)
- [Authentication](#authentication)
- [Asynchronous Job Processing](#asynchronous-job-processing)
- [Audit Logging](#audit-logging)
- [Monitoring & Health Checks](#monitoring--health-checks)
- [AI Integration](#ai-integration)
- [Bonus Features](#bonus-features)
- [Testing](#testing)
- [Swagger / OpenAPI Docs](#swagger--openapi-docs)

---

## Features

✅ Upload CVs in **PDF or DOCX** format  
✅ Extract and store **plain text** from uploaded CVs  
✅ Integrate **OpenAI GPT** or **ML.NET** to extract:
- Technical skills
- Work experience
- Education

✅ Match CVs with multiple job descriptions  
✅ Score compatibility and generate **AI-based explanations**  
✅ Run tasks **asynchronously** with background processing (Hangfire)  
✅ Track job status and retrieve results by ID  
✅ Use **JWT authentication** and **operation audit logging**  
✅ Monitor system usage, user activity, and health  
✅ Provide AI-generated **CV improvement tips**  

---

## Tech Stack

- **Language**: C#
- **Framework**: ASP.NET Core Web API
- **AI Services**: OpenAI API or ML.NET
- **Background Jobs**: Hangfire
- **Auth**: JWT Bearer Tokens
- **Monitoring**: Application Insights / Prometheus + Grafana (optional)
- **API Documentation**: Swagger / OpenAPI

---

## API Endpoints

### 📤 CV Upload

**POST** `/api/cv/upload`

- Accepts a file (PDF or DOCX)
- Extracts and stores text content
- Returns `uploadId`

**Request (multipart/form-data):**
```http
Content-Type: multipart/form-data

form-data:
- file: resume.pdf
```

**Response:**
```json
{
  "uploadId": "123e4567-e89b-12d3-a456-426614174000"
}
```

---

### 🧠 Start CV Analysis

**POST** `/api/cv/analyze`

- Initiates analysis for the uploaded CV
- Takes a list of job descriptions
- Returns a `resultId` for tracking

**Request:**
```json
{
  "analyzeId": "123e4567-e89b-12d3-a456-426614174000",
  "jobDescriptions": [
    "Looking for a .NET Developer with experience in REST APIs...",
    "Full-stack engineer with React and C# background..."
  ],
  "language": "en"
}
```

**Response:**
```json
{
  "resultId": "aa4c24a6-49b0-4c3a-ae12-fb179cbe1aef",
  "status": "Processing"
}
```

---

### 📊 Get Analysis Result

**GET** `/api/cv/result/{resultId}`

- Fetch the result of the CV-to-job matching
- Returns AI-extracted data and compatibility scores

**Response:**
```json
{
  "cvDetails": {
    "skills": ["C#", ".NET", "SQL"],
    "experience": ["5 years at XYZ", "2 years freelance"],
    "education": ["BSc in Computer Science"]
  },
  "matches": [
    {
      "jobIndex": 0,
      "score": 85,
      "explanation": "Strong match for backend requirements."
    },
    {
      "jobIndex": 1,
      "score": 60,
      "explanation": "Lacks front-end expertise in React."
    }
  ],
  "status": "Completed"
}
```

---

### 📈 Analytics & Monitoring

#### API Usage Stats

**GET** `/api/analytics/usage-stats`

Returns total usage metrics:
```json
{
  "totalCVsUploaded": 120,
  "totalAnalyses": 95,
  "avgAnalysisTime": "3.5s"
}
```

#### Per-User Stats

**GET** `/api/analytics/user-stats`

Requires Authorization

```json
{
  "userId": "user123",
  "uploads": 10,
  "analyses": 8,
  "topFunction": "CV Analysis"
}
```

#### Health Check

**GET** `/api/health`

```json
{
  "status": "Healthy",
  "uptime": "72h",
  "database": "Connected",
  "aiService": "Online"
}
```

---

## Authentication

All endpoints (except health and public Swagger) are protected with **JWT Bearer Tokens**.

Add the token to requests:

```http
Authorization: Bearer {your_token}
```

---

## Asynchronous Job Processing

- All AI-related processing is handled in the background using **Hangfire**.
- Each job is assigned a unique `resultId`.
- Users can check job progress using `/api/cv/result/{resultId}`.

---

## Audit Logging

- Every user action (upload, analysis, result retrieval) is logged with:
  - Timestamp
  - User ID
  - Operation
  - Status / Error (if any)

---

## Monitoring & Health Checks

- Built-in health check endpoint `/api/health`
- Optional integration with:
  - **Prometheus + Grafana**
  - **Application Insights**
- Custom middleware tracks API response time, exceptions, and usage patterns.

---

## AI Integration

- Uses **OpenAI GPT** or **ML.NET** (configurable)
- Extracts structured data from free-form CVs
- Compares content with job descriptions using vector similarity and NLP scoring
- Generates plain-English explanations for each compatibility score

---

## Bonus Features

### 💡 CV Improvement Tips

**GET** `/api/cv/improvement-tips/{uploadId}`

Returns AI-generated suggestions for improving the uploaded CV.

**Example:**
```json
{
  "tips": [
    "Add more quantifiable achievements in your experience section.",
    "Consider including relevant keywords like '.NET Core' and 'microservices'."
  ]
}
```

---

## Testing

- Includes sample **unit** and **integration tests** for:
  - Upload validation
  - Background job enqueuing
  - AI processing pipeline
- Recommend using **xUnit** and **Moq** for mocking services

---

## Swagger / OpenAPI Docs

- Full interactive documentation is available via Swagger UI:
  - **`/swagger/index.html`**

---

## License

This project is intended for educational and prototype use. Commercial use may require licensing of AI APIs and tools (e.g., OpenAI).
