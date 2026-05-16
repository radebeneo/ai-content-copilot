# AI Content Copilot for Umbraco

> An AI-powered content assistant built into the Umbraco CMS backoffice — helping editors generate titles, rewrite copy, summarise content, and create meta descriptions in seconds.

---

## Table of Contents

- [Overview](#overview)
- [Features](#features)
- [Tech Stack](#tech-stack)
- [Project Structure](#project-structure)
- [Getting Started](#getting-started)
    - [Prerequisites](#prerequisites)
    - [Installation](#installation)
    - [Configuration](#configuration)
    - [Running the Project](#running-the-project)
- [API Reference](#api-reference)
- [Usage](#usage)
- [Architecture](#architecture)
- [Development Timeline](#development-timeline)
- [Non-Goals](#non-goals)
- [Contributing](#contributing)
- [License](#license)

---

## Overview

AI Content Copilot is a lightweight CMS extension that embeds an AI assistant directly inside the Umbraco backoffice. Content editors can highlight or paste source copy, choose an action, and receive AI-generated output — without ever leaving the CMS.

The tool is designed to feel like a natural part of the editorial workflow: practical, fast, and brand-safe. It is not a chatbot or a marketing automation suite — it is a focused, production-minded writing aid.

**Target users:**
- Content editors
- Marketing team members
- Website administrators
- Digital agency staff managing client content

---

## Features

### MVP (Current)
| Feature | Description |
|---|---|
| **Title Generation** | Generates 3 concise, compelling page titles from source content |
| **Content Rewrite** | Rewrites copy in a selected tone (professional, friendly, concise) |
| **Summarise** | Condenses long content into a 2–3 sentence summary |
| **Meta Description** | Produces an SEO-friendly meta description under 160 characters |
| **Editable Output** | All generated text is returned in an editable textarea |
| **Copy to Clipboard** | One-click copy of any generated result |
| **CMS Panel UI** | Fixed sidebar panel rendered directly inside the Umbraco backoffice |

### Planned (Nice to Have)
- Tone selector with extended options (e.g. formal, humorous)
- Output length control (short / medium / long)
- Undo / version history per session
- Prompt templates scoped to specific Document Types

---

## Tech Stack

| Layer | Technology                                     |
|---|------------------------------------------------|
| **CMS** | [Umbraco](https://umbraco.com/) (ASP.NET Core) |
| **Backend** | C# / .NET                                      |
| **AI Provider** | [Gemini API](https://docs.anthropic.com/)      |
| **Frontend** | Vanilla JS + Razor partial view (`.cshtml`)    |
| **HTTP Client** | `IHttpClientFactory` (named client)            |
| **IDE** | JetBrains Rider                                |

---

## Project Structure

```
MyProject/
├── Controllers/
│   └── AiCopilotController.cs      # API endpoints for all generation actions
├── Models/
│   ├── AiCopilotSettings.cs        # Strongly-typed config binding
│   └── ContentRequest.cs           # Request body model
├── Services/
│   ├── AnthropicService.cs         # HTTP client wrapper for the Claude API
│   └── PromptTemplates.cs          # All prompt definitions (centralised)
├── Views/
│   ├── Partials/
│   │   └── AiCopilot.cshtml        # Copilot sidebar panel (UI + JS)
│   └── Shared/
│       └── _Layout.cshtml          # Main layout (panel rendered here)
├── wwwroot/
├── AiCopilot.http                  # Rider HTTP client test file
├── appsettings.json                # App configuration (no secrets committed)
└── Program.cs                      # Service registration and pipeline setup
```

---

## Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/en-us/download) or later
- [JetBrains Rider](https://www.jetbrains.com/rider/) (recommended) or Visual Studio 2022+
- An [Anthropic API key](https://console.anthropic.com/)
- Umbraco templates installed via the .NET CLI (`dotnet new umbraco`)

### Installation

1. **Clone the repository**

   ```bash
   git clone https://github.com/your-username/ai-content-copilot.git
   cd ai-content-copilot
   ```

2. **Restore dependencies**

   ```bash
   dotnet restore
   ```

3. **Install required NuGet packages** (if not already present)

   ```bash
   dotnet add package Microsoft.Extensions.Http
   ```

### Configuration

**Option A — User Secrets (recommended for local development)**

```bash
dotnet user-secrets init
dotnet user-secrets set "AiCopilot:GeminiApiKey" "ai-gem-your-key-here"
```

**Option B — `appsettings.json`**

Add the following block to `appsettings.json`. Do **not** commit this file if it contains a real API key.

```json
{
  "AiCopilot": {
    "GeminiApiKey": "ai-gem-your-key-here",
    "Model": "gemini-1.5-flash",
    "MaxTokens": 1000
  }
}
```

> ⚠️ Ensure `appsettings.json` is listed in `.gitignore` if it contains credentials.

### Running the Project

```bash
dotnet run
```

Then open:
- `https://localhost:{PORT}` — front end
- `https://localhost:{PORT}/umbraco` — Umbraco backoffice

Complete the Umbraco setup wizard on first run (create an admin account and select a starter kit or blank install).

---

## API Reference

All endpoints accept `POST` requests with a JSON body and return a JSON object containing a `result` string.

**Base URL:** `/api/ai-copilot`

---

### `POST /generate-title`

Generates three page title options from source content.

**Request body:**
```json
{
  "content": "Your source content or brief here."
}
```

**Response:**
```json
{
  "result": "1. Title One\n2. Title Two\n3. Title Three"
}
```

---

### `POST /rewrite`

Rewrites content in a specified tone.

**Request body:**
```json
{
  "content": "Original copy to rewrite.",
  "tone": "professional"
}
```

> Accepted tone values: `professional`, `friendly`, `concise`

---

### `POST /summarize`

Condenses content into 2–3 sentences.

**Request body:**
```json
{
  "content": "Long-form content to summarise."
}
```

---

### `POST /meta-description`

Generates an SEO meta description under 160 characters.

**Request body:**
```json
{
  "content": "Page content to base the meta description on."
}
```

---

**Error responses** follow this shape:

```json
{
  "error": "Descriptive error message."
}
```

| Status Code | Meaning |
|---|---|
| `400` | Missing or empty `content` field |
| `500` | Anthropic API call failed or unexpected server error |

---

## Usage

Once the project is running and the backoffice is open:

1. Navigate to any content node in the Umbraco backoffice.
2. The **AI Copilot panel** appears as a fixed sidebar on the right-hand side.
3. Paste your source content or a brief into the input area.
4. Select a tone from the dropdown (relevant for the **Rewrite** action).
5. Click one of the four action buttons.
6. Review the generated output in the editable text area.
7. Click **Copy to Clipboard** or manually paste the result into your CMS field.

---

## Architecture

```
Umbraco Backoffice (browser)
        │
        │  POST /api/ai-copilot/{action}
        ▼
AiCopilotController.cs
        │
        │  Selects prompt template
        ▼
PromptTemplates.cs  ──────►  Structured prompt string
        │
        ▼
GeminiService.cs
        │
        │  POST 
        ▼
Gemini API  ──────►  Generated text
        │
        ▼
Controller returns { result }
        │
        ▼
AiCopilot.cshtml (JS)  ──────►  Renders in output textarea
```

**Key design decisions:**

- **Prompts are centralised** in `PromptTemplates.cs` so they can be tuned independently of controller logic.
- **`IHttpClientFactory`** is used for the Gemini client to support connection pooling and avoid socket exhaustion.
- **Strongly-typed settings** via `IOptions<AiCopilotSettings>` keep configuration clean and testable.
- **No raw API complexity** is exposed to the frontend — editors interact only with action buttons and text areas.

---

## Development Timeline

| Week | Focus | Key Deliverables                                                      |
|---|---|-----------------------------------------------------------------------|
| **Week 1** | Setup & planning | Project structure, environment config, user flow definition           |
| **Week 2** | Backend integration | `AiService`, `PromptTemplates`, all 4 API endpoints, endpoint testing |
| **Week 3** | UI & error handling | Copilot panel partial view, frontend JS, structured error responses   |
| **Week 4** | Polish & demo | Logging, UX improvements, documentation, demo recording               |

---

## Non-Goals

The following are explicitly out of scope for this project:

- Full marketing automation
- Advanced SEO scoring
- Multi-language content support
- Complex workflow approvals
- Full chatbot functionality

---

## Contributing

1. Fork the repository and create a feature branch (`git checkout -b feature/your-feature`).
2. Keep prompts in `PromptTemplates.cs` — do not hardcode them in controllers.
3. Follow the existing error handling pattern (tuple return from `AiService`).
4. Test new endpoints using the `AiCopilot.http` file before wiring up UI.
5. Open a pull request with a clear description of what changed and why.

---

## License

This project is licensed under the [MIT License](LICENSE).

---

*Built with C# and Umbraco.*