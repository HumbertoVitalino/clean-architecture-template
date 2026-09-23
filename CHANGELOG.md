# Changelog

All notable changes to this template are documented in this file.

## [Unreleased]

### Added

- Structured logging with Serilog, replacing the default ASP.NET Core logger, with a consistent console output format across the API.
- OpenTelemetry instrumentation for tracing (ASP.NET Core, HttpClient, Npgsql) and metrics (ASP.NET Core, HttpClient, runtime, Npgsql), exported via the console exporter.
- Correlation id support: bound from the `X-Correlation-Id` request header (or generated when absent) into every use case `Input`, logged manually as a `[{CorrelationId}] | ...` prefix on error paths.

### Changed

- Removed the correlation-id middleware in favor of binding `X-Correlation-Id` directly at each endpoint, so the id used in logs always matches the one carried by the use case `Input` (previously the middleware could generate a different id than the endpoint when the header was missing).
