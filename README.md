# media-api

**media-api** is a **.NET RESTful API** for managing media files.  
It is designed as a lightweight file service for cases where you want to host and manage files without building a full backend.

The API supports **most file types**, including:

- Images  
- Videos  
- PDFs  
- ZIP files  
- Any other files you want to host online  

(anything but .exe)
---

## Architecture

The API is built using **Onion Architecture**, keeping domain logic isolated from infrastructure and framework concerns and making the codebase easier to maintain and extend.

---

## Ownership Tokens

The system uses **Ownership Tokens**.

- Tokens are generated and reused across the application  
- They behave more like **API keys** than modern JWTs  
- Uploaded files are linked to the token used during upload  
- The same token is required to **find**, **modify**, or **delete** those files  

---

## Health Endpoint
The API exposes a health endpoint:
This endpoint provides runtime information about the API, such as uptime and whether the database is running.

---

## Miscellaneous

- **Unit tests** are included to validate behavior and help keep the system stable as it evolves  
- **File storage is platform-independent**, allowing the API to run on different systems without storage-specific assumptions
- The API itself is **hosted on Linux** [api.media.syter6.nl](https://api.media.syter6.nl/swagger)
- There are **zero compiler warnings**  
*(Yes. That took a while.)*
