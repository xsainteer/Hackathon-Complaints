# GovComplaints

**GovComplaints** is a centralized platform for collecting and analyzing complaints, suggestions, and feedback directed at government agencies and their local branches.

## 🚩 Problem

### For Citizens
- Each government agency collects complaints independently, forcing people to search and submit feedback across separate systems.
- Many citizens are unaware of the ability to submit complaints electronically.

### For Government Agencies
- Repeated complaints are not grouped or analyzed semantically.
- Manual complaint review is slow and ineffective.
- Incomplete or vague submissions reduce response efficiency.

## ✅ Solution

- A unified web platform that centralizes feedback across all agencies.
- Unique QR codes for each agency branch to simplify submission.
- Built-in analytics dashboard for administrators.
- Location and demographic data collection.
- AI-powered semantic search and summarization using embeddings.

## 🧠 Key Features

- Complaint submission interface  
- Admin panel for responding to complaints  
- Semantic search through submitted complaints  
- Automatic clustering of frequent complaint topics  
- QR code generator for each agency branch  
- Analytics panel tracking QR scans and engagement  

## 🛠️ Tech Stack

- **ASP.NET Core** – Web API backend  
- **PostgreSQL** – Relational database  
- **Qdrant** – Vector search engine for semantic data  
- **React** – Frontend UI  
- **Ollama** – LLM for embeddings and summarization  

## 📅 Status

MVP built during a university hackathon. Active development continues.

## 🔗 Links

- [📎 KSTU Hackathon page](https://kstu.kg/bokovoe-menju/ehndaument-fond-kgtu/novosti?tx_news_pi1%5Baction%5D=detail&tx_news_pi1%5Bcontroller%5D=News&tx_news_pi1%5Bnews%5D=5488&cHash=574622b21fd6150453823a5484cfe0b3)
- [📎 Miro — User & Admin Journey](https://miro.com/app/board/uXjVIvSgtJY=/?share_link_id=184250474826)
