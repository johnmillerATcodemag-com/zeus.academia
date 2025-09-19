---
mode: 'agent'
model: Claude Sonnet 4
tools: ['githubRepo', 'codebase']
description: 'Generate instruction files for a CQRS application architecture in an Academic Management System.'
---
# Instructions Generation Prompt

## Objective
Generate a cohesive, complete set of instruction documents for implementing a CQRS-based Academic Management System (Academics, Departments, Roles, etc.) grounded in Object Role Modeling (ORM) principles. Use the ORM white paper for modeling guidance: https://orm.net/pdf/ORMwhitePaper.pdf

## Your Task
Produce the following instruction files exactly once, and nothing else. Each file must be complete, self-contained, and follow the Output Formatting and Response Rules below.

## Deliverables (Exact Output Files)
1. .github/instructions/generated/project.instructions.md
2. .github/instructions/generated/sdlc.instructions.md
3. .github/instructions/generated/git-hygiene.instructions.md
4. .github/instructions/generated/cqrs-guidelines.instructions.md
5. .github/instructions/generated/architecture.instructions.md
6. .github/instructions/generated/azure-provisioning.instructions.md
7. .github/instructions/generated/secrets-and-config.instructions.md
8. .github/instructions/generated/testing-strategy.instructions.md
9. .github/instructions/generated/documentation-standards.instructions.md
10. .github/instructions/generated/observability.instructions.md
11. .github/instructions/generated/security-and-compliance.instructions.md
12. .github/instructions/generated/operations-and-deployment.instructions.md
13. .github/instructions/generated/ai-generated-code-policy.instructions.md
14. .github/instructions/generated/data-and-migrations.instructions.md
15. .github/instructions/generated/glossary.instructions.md

## Global Principles
- Treat all compiler/analyzer warnings as errors; no merge until resolved.
- Enforce formatting & linting (define tools, consistent style).
- Human review required for all AI-generated code (branch isolation).
- Deterministic, reproducible builds (pin versions).
- Infrastructure as Code only (no ad-hoc portal changes).
- Security & compliance embedded (shift-left scanning).
- Explicit ownership and change governance for instruction files.

## Project Instructions
- Define purpose, scope, and high-level architecture of the Academic Management System.
- Directory Structure: src/, tests/, docs/, infra/, scripts/, .github/.
- Tech Stack: .NET 7, C#, Azure SQL, Azure Functions, GitHub Actions, OpenTelemetry.
- Branching Strategy: GitFlow or trunk-based (justify choice).
- CI/CD: Linting, build, test, security scans, deploy to dev/test/stage/prod.
- Code Reviews: At least one approval; checklist includes coding standards, tests, security.

## CQRS Requirements
- Commands: imperative, change state; idempotent where applicable.
- Queries: side-effect free, support pagination/filtering/sorting.
- Handlers: one per command/query; enforce business invariants in domain layer.
- Separate read models from write models.
- Eventual consistency patterns: latency expectations, retry/backoff.
- Guidance on when to introduce domain events vs. keep transactional.

## Architecture Documentation
- Provide C4 diagrams (Mermaid): Context, Container, Component, Deployment.
- Include sequence diagrams for: CreateEmployee, AssignRole, QueryDepartmentRoster.
- Document boundaries, aggregates, invariants.

## Azure Provisioning
- Default Region: WestUS2 (justify or document any override).
- Resource group naming convention.
- Resources: App runtime (App Service or Functions), Database (Azure SQL or Cosmos DB—justify), Storage, Key Vault, Monitor (Logs, Metrics, App Insights), CI/CD (GitHub Actions).
- Tagging: env, owner, cost-center, data-classification.
- Blueprint for dev/test/stage/prod with isolation policy.

## Secrets & Configuration
- All secrets in Key Vault; reference in pipelines.
- Configuration precedence: environment → Key Vault → defaults.
- Rotation policy & auditing.

## Testing Strategy
- Unit coverage: ≥ 80% line / ≥ 70% branch.
- Integration: API contract, status codes, schema validation.
- Contract tests (if external consumers).
- E2E for critical user journeys.
- Post-deploy smoke tests.
- Performance: baseline latencies, throughput, error budget.
- Mutation testing target: ≥ 60% survived reduction.
- Enforce coverage gates in CI.

## Observability
- Structured JSON logging, correlation IDs.
- OpenTelemetry traces for command and query handlers.
- Metrics: command latency, handler failures, DB query timings, queue lag (if messaging).
- Alerts: error rate > threshold, p95 latency breaches, failed deployments.

## Security & Compliance
- Threat modeling (STRIDE) per release.
- RBAC mapping: roles vs. domain permissions.
- Input validation + centralized exception mapping (RFC 9457 problem+json).
- Dependency, SAST, secret, license scanning each pipeline run.

## Operations & Deployment
- Blue-green or canary optional; define rollback strategy.
- Migration strategy: forward-only with pre-flight validation.
- Backup policy with documented RPO/RTO targets.

## AI-Generated Code Policy
- PRs must include provenance tag.
- Explicit reviewer approval checklist for AI-generated changes.

## Data & Migrations
- Naming conventions, migration ordering, seeding strategy.
- Handling breaking read model changes (versioned projections).

## Documentation Standards
- README includes: purpose, architecture snapshot, setup, troubleshooting.
- Mermaid diagrams auto-regeneration script (describe exact command).
- Change log follows “Keep a Changelog”.

## Acceptance Criteria Template
Given ...
When ...
Then ...

## Git Hygiene Instructions
- Commit messages: conventional commits (feat, fix, docs, style, refactor, test, chore), optional scope, concise description.
- Branch names: lowercase, hyphens, clear purpose (e.g., feature/user-authentication, bugfix/login-error).
- PRs: detailed description, rationale, issue links.
- Reviews: at least one approval before merge.
- Use “Squash and Merge”.
- Resolve conflicts promptly; communicate as needed.
- Delete branches after merge.
- Workflow implementations must be shown to work before merging to main.

## Glossary
- Define: Aggregate, Command, Query, Read Model, Projection, Invariant, Idempotency, etc.

## Output Formatting and Response Rules
- Generate all files exactly once, in the order listed under Deliverables; no extraneous commentary.
- Each file starts with Title, Purpose, Scope.
- Each file ends with a Compliance Checklist.
- No placeholder text; include concrete examples for at least one Employee command & query.
- Use Mermaid code blocks for diagrams.
- Ensure consistency with Global Principles across all files.


