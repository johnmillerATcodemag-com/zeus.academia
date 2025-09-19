---
mode: 'agent'
model: Claude Sonnet 4
tools: ['githubRepo', 'codebase']
description: 'Generate instruction files for a CQRS application architecture in a Academic Management System.'
---
# Instructions Generation Prompt

## Goal
This prompt is intended to Generate a cohesive set of instruction files for implementing a CQRS-based Academic Management System (Academics, Departments, Roles, etc.) grounded in Object Role Modeling principles that supports storing and retrieving data in conformance with the business rules.

The model is based on this [ORM white paper](https://orm.net/pdf/ORMwhitePaper.pdf), which provides a structured approach to object role modeling.

## Required Output Files
1. .github/instructions/generated/project.instructions.md
1. .github/instructions/generated/sdlc.instructions.md
1. .github/instructions/generated/git-hygiene.instructions.md
1. .github/instructions/generated/cqrs-guidelines.instructions.md
1. .github/instructions/generated/architecture.instructions.md
1. .github/instructions/generated/azure-provisioning.instructions.md
1. .github/instructions/generated/secrets-and-config.instructions.md
1. .github/instructions/generated/testing-strategy.instructions.md
1. .github/instructions/generated/documentation-standards.instructions.md
1. .github/instructions/generated/observability.instructions.md
1. .github/instructions/generated/security-and-compliance.instructions.md
1. .github/instructions/generated/operations-and-deployment.instructions.md
1. .github/instructions/generated/ai-generated-code-policy.instructions.md
1. .github/instructions/generated/data-and-migrations.instructions.md
1. .github/instructions/generated/data-and-migrations.instructions.md
1. .github/instructions/generated/glossary.instructions.md

## Global Principles
- Treat all compiler/analyzer warnings as errors.
  - Code should not be merged to main until all errors and warnings are resolved.
- Enforce formatting & linting (define tools).
  - Include consistent indentation, naming conventions, and overall code style.
- Human review required for all AI-generated code (branch isolation).
- Deterministic, reproducible builds (pin versions).
- Infrastructure as Code only (no ad-hoc portal changes).
- Security & compliance embedded (shift-left scanning).
- Explicit ownership and change governance for instruction files.

## Project instructions
- Instructions that are specific to this project
    - Define the purpose, scope, and high-level architecture of the Academic Management System.
    - Directory Structure: src/, tests/, docs/, infra/, scripts/, .github/.
    - Tech Stack: .NET 7, C#, Azure SQL, Azure Functions, GitHub Actions, OpenTelemetry.
    - Branching Strategy: GitFlow or trunk-based (justify choice).
    - CI/CD: Linting, build, test, security scans, deploy to dev/test/stage/prod.
    - Code Reviews: At least one approval required; checklist includes coding standards, tests, security

## CQRS Requirements
- Commands: imperative, change state; must be idempotent where applicable.
- Queries: side-effect free, support pagination/filtering/sorting.
- Handlers: one per command/query; business invariants enforced in domain layer.
- Read models separated from write models.
- Eventual consistency patterns described (latency expectations, retry/backoff).
- Optional: define when to introduce domain events vs. keep transactional.

## Architecture Documentation
- Provide C4: Context, Container, Component, Deployment diagrams (Mermaid).
- Include sequence diagrams for key flows: CreateEmployee, AssignRole, QueryDepartmentRoster.
- Document boundaries, aggregates, invariants.

## Azure Provisioning
- Region: WestUS2 (justify / override rules).
- Resource group naming convention.
- Resources: App runtime (e.g., App Service or Functions), Database (Azure SQL or Cosmos DB—justify choice), Storage, Key Vault, Monitor (Logs, Metrics, App Insights), CI/CD (GitHub Actions).
- Tagging: env, owner, cost-center, data-classification.
- Blueprint for dev/test/stage/prod with isolation policy.

## Secrets & Configuration
- All secrets in Key Vault; reference in pipelines.
- Configuration precedence: environment → Key Vault → defaults.
- Rotation policy & auditing.

## Testing Strategy
- Unit (80% line / 70% branch minimum).
- Integration (API contract, status codes, schema validation).
- Contract tests (if external consumers).
- E2E (critical user journeys).
- Smoke (post-deploy).
- Performance (baseline latencies, throughput, error budget).
- Mutation testing target ≥ 60% survived reduction.
- Coverage gates enforced in CI.

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
- Blue-green or canary deployment optional; rollback strategy defined.
- Migration strategy: forward-only with pre-flight validation.
- Backup policy + RPO/RTO targets documented.

## AI-Generated Code Policy
- Must include provenance tag in PR description.
- Requires explicit reviewer approval checklist.

## Data & Migrations
- Naming conventions, migration ordering, seeding strategy.
- Handling breaking read model changes (versioned projections).

## Documentation Standards
- README includes: purpose, architecture snapshot, setup, troubleshooting.
- Mermaid diagrams auto-regenerated script (describe command).
- Change log (Keep a Changelog format).

## Acceptance Criteria Template
Given ...
When ...
Then ...

## Git Hygiene Instructions
- Commit messages must be descriptive and follow the conventional commit format. This includes a type (feat, fix, docs, style, refactor, test, chore), an optional scope, and a concise description of the change.
- Branch names should be clear and follow a consistent naming convention. This includes using lowercase letters, hyphens to separate words, and including relevant information such as the feature or bug being addressed (e.g., feature/user-authentication, bugfix/login-error). Consistent branch naming improves organization and makes it easier to identify the purpose of each branch.
- Pull requests must include a detailed description of the changes made, the reason for the changes, and any relevant issue numbers. This helps reviewers understand the context of the changes and facilitates the review process.
- Pull requests should be reviewed and approved by at least one other team member before merging. This ensures that code quality is maintained and that multiple perspectives are considered.
- Merge pull requests using the "Squash and Merge" option to keep the commit history clean and concise. This combines all commits from the feature branch into a single commit on the main branch, making it easier to track changes.
- Resolve any merge conflicts promptly and communicate with the team if assistance is needed. This helps prevent delays in the development process and ensures that everyone is aware of changes being made.
- Delete branches after merging to keep the repository clean and organized. This prevents clutter and reduces confusion about which branches are active.
- Workflow implementations must be shown to work before merging to main

## Glossary
- Define: Aggregate, Command, Query, Read Model, Projection, Invariant, Idempotency, etc.

## Output Formatting Rules
- Each file starts with Title, Purpose, Scope.
- Ends with a Compliance Checklist.
- No placeholder text; use concrete examples for at least one Employee command & query.

Generate all files exactly once, no extraneous commentary.


