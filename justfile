default:
    @just --list

dev:
    @just dev-backend & just dev-tessa & wait

dev-tessa:
    cd tessa && dotnet watch

dev-build:
    @just dev-backend & cd tessa && dotnet run & wait

dev-backend:
    cd backend && uv run uvicorn main:app --reload
