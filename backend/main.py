from fastapi import FastAPI
from fastapi.responses import StreamingResponse
from src.utils import *
import io

app = FastAPI()

@app.get("/")
async def root():
    return {"message": "status: works"}

@app.post("/test")
async def get_tessdata(payload: dict):
    print(f"Payload: {payload}")
    document = create_doc(payload['texts'])

    buffer = io.BytesIO()
    document.save(buffer)
    buffer.seek(0)

    return StreamingResponse(
        buffer,
        media_type="application/vnd.openxmlformats-officedocument.wordprocessingml.document",
        headers={
            "Content-Disposition": "attachment; filename=Tessa_Generated.docx"
        }
    )
