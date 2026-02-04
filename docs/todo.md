# TODO

## Findings vs SRS (gaps):

- CORS falls back to allow any origin when no config is set, which violates “only approved client domains.” See Program.cs.
- Login failures return 401 without a meaningful message; SRS asks for a relevant error message. See AuthEndpoints.cs.
- JWT key has a weak default fallback; SRS requires a secure signing key. See Program.cs and JwtTokenService.cs.
- Logging/auditing of important events isn’t implemented beyond defaults; SRS wants logging for errors/warnings/important events. See Program.cs.
- Mongo indexes only cover userId + createdAt; filters on status, priority, and dueDate may not meet the <200ms target under load. See MongoDbContext.cs.

## Todo

### Next steps

- [ ] Require explicit CORS origins (no AllowAnyOrigin fallback).
- [ ] Return a clear auth error message for failed login.
- [ ] Enforce secure JWT key in production (no weak default fallback).
- [ ] Add MongoDB indexes for common filters/sorts (status, priority, dueDate per user).
- [ ] Add structured logging for auth and todo mutations.
- [ ] Add tests for due-date filter/sort and forbidden access.
