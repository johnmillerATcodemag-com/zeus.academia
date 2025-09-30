# Git Workflow Guidelines

## Purpose

This document establishes standardized Git workflow practices for the Academic Management System, ensuring consistent branching strategies, commit message formatting, pull request procedures, and merge policies that support collaborative development and maintain code quality.

## Scope

This document covers:

- Branch naming conventions and lifecycle management
- Commit message standards and formatting rules
- Pull request templates and review procedures
- Merge strategies and conflict resolution
- Release management and hotfix procedures

This document does not cover:

- Git installation and configuration procedures
- IDE-specific Git integration details
- Advanced Git troubleshooting techniques
- Repository hosting platform administration

## Prerequisites

- Basic understanding of Git version control concepts
- Familiarity with feature branch workflows
- Knowledge of pull request/merge request processes
- Understanding of semantic versioning principles

## Branch Management Strategy

### Branch Types and Naming Conventions

#### Main Branches

- **`main`**: Production-ready code, always deployable
- **`develop`**: Integration branch for features, pre-release testing

#### Supporting Branches

- **Feature branches**: `feature/ACA-123-add-student-enrollment`
- **Bug fix branches**: `bugfix/ACA-456-fix-grade-calculation`
- **Hotfix branches**: `hotfix/ACA-789-critical-security-patch`
- **Release branches**: `release/v1.2.0`

### Branch Naming Rules

```bash
# Feature branches
feature/[TICKET-ID]-[short-description]
feature/ACA-123-implement-course-enrollment
feature/ACA-124-add-grade-management

# Bug fix branches
bugfix/[TICKET-ID]-[short-description]
bugfix/ACA-456-fix-null-reference-exception
bugfix/ACA-457-resolve-enrollment-validation

# Hotfix branches
hotfix/[TICKET-ID]-[short-description]
hotfix/ACA-789-security-vulnerability-patch
hotfix/ACA-790-critical-data-corruption-fix

# Release branches
release/v[MAJOR].[MINOR].[PATCH]
release/v1.2.0
release/v2.0.0
```

### Branch Lifecycle Management

#### Feature Branch Workflow

```mermaid
gitgraph
    commit id: "Initial"
    branch develop
    checkout develop
    commit id: "Dev baseline"
    branch feature/ACA-123
    checkout feature/ACA-123
    commit id: "Add enrollment entity"
    commit id: "Add business rules"
    commit id: "Add unit tests"
    checkout develop
    merge feature/ACA-123
    commit id: "Feature complete"
    checkout main
    merge develop
    commit id: "Release v1.1.0"
```

#### Creating Feature Branches

```bash
# Update local main branch
git checkout main
git pull origin main

# Create and switch to feature branch
git checkout -b feature/ACA-123-implement-enrollment

# Set upstream tracking
git push -u origin feature/ACA-123-implement-enrollment
```

#### Feature Development Workflow

```bash
# Regular commits during development
git add .
git commit -m "feat: add student enrollment validation rules

- Implement credit hour limits validation
- Add prerequisite checking logic
- Include enrollment period validation

Resolves: ACA-123"

# Push changes regularly
git push origin feature/ACA-123-implement-enrollment

# Keep feature branch updated with main
git checkout main
git pull origin main
git checkout feature/ACA-123-implement-enrollment
git rebase main

# Resolve any conflicts and continue
git add .
git rebase --continue
git push --force-with-lease origin feature/ACA-123-implement-enrollment
```

## Commit Message Standards

### Commit Message Format

```
<type>(<scope>): <subject>

<body>

<footer>
```

### Commit Types

- **feat**: New feature implementation
- **fix**: Bug fix
- **docs**: Documentation changes
- **style**: Code style changes (formatting, missing semicolons, etc.)
- **refactor**: Code refactoring without feature changes
- **test**: Adding or modifying tests
- **chore**: Build process or auxiliary tool changes
- **perf**: Performance improvements
- **ci**: Continuous integration changes

### Commit Message Examples

#### Feature Implementation

```
feat(enrollment): implement student course enrollment

Add comprehensive enrollment validation including:
- Credit hour limit enforcement (max 21 credits/term)
- Prerequisite course verification
- Schedule conflict detection
- Enrollment period validation

The enrollment process now follows business rules defined
in the academic policy document section 4.2.

Resolves: ACA-123
```

#### Bug Fix

```
fix(grading): resolve grade calculation rounding error

Fix decimal precision issue in GPA calculations that was
causing incorrect grade point averages. Changed from
float to decimal type to maintain precision.

Before: 3.67 GPA displayed as 3.6699998
After: 3.67 GPA displayed correctly as 3.67

Resolves: ACA-456
```

#### Breaking Change

```
feat(api)!: change enrollment endpoint response format

BREAKING CHANGE: The enrollment API response format has
changed to include additional metadata fields.

Old format:
{
  "success": true,
  "enrollmentId": "123"
}

New format:
{
  "success": true,
  "data": {
    "enrollmentId": "123",
    "enrollmentDate": "2024-01-15T10:00:00Z",
    "academicTerm": "FALL2024"
  },
  "metadata": {
    "version": "v2",
    "timestamp": "2024-01-15T10:00:00Z"
  }
}

Resolves: ACA-789
```

### Commit Message Validation

```bash
#!/bin/bash
# .gitmessage template
# Type(<scope>): <subject> (max 50 characters)
#
# <body> (wrap at 72 characters)
#
# Resolves: <ticket-number>
# Co-authored-by: Name <email@example.com>
```

## Pull Request Process

### Pull Request Template

```markdown
## Description

Brief description of changes and motivation for the change.

## Type of Change

- [ ] Bug fix (non-breaking change which fixes an issue)
- [ ] New feature (non-breaking change which adds functionality)
- [ ] Breaking change (fix or feature that would cause existing functionality to not work as expected)
- [ ] Documentation update
- [ ] Performance improvement
- [ ] Code refactoring

## Related Tickets

- Resolves: ACA-123
- Related: ACA-124, ACA-125

## Testing Performed

- [ ] Unit tests added/updated and passing
- [ ] Integration tests added/updated and passing
- [ ] Manual testing completed
- [ ] Performance testing completed (if applicable)

## Test Coverage

- Unit test coverage: 87% (minimum 85% required)
- Integration test coverage: 72% (minimum 70% required)

## Breaking Changes

Describe any breaking changes and migration steps:

- [ ] API contract changes documented
- [ ] Database migration scripts included
- [ ] Deployment instructions updated

## Screenshots/Evidence

Include screenshots, logs, or other evidence of functionality if applicable.

## Checklist

- [ ] Code follows project style guidelines
- [ ] Self-review of code completed
- [ ] Code is commented, particularly in hard-to-understand areas
- [ ] Corresponding documentation updated
- [ ] No new warnings introduced
- [ ] Tests added that prove fix is effective or feature works
- [ ] New and existing tests pass locally
- [ ] Dependent changes merged and published
```

### Pull Request Requirements

#### Automated Checks

- **Build Status**: All builds must pass
- **Test Coverage**: Minimum 85% unit test coverage, 70% integration coverage
- **Code Analysis**: No critical or high severity issues
- **Security Scan**: No high or critical vulnerabilities
- **Performance**: No significant performance regressions detected

#### Review Requirements

```yaml
# .github/CODEOWNERS
# Global reviewers for all changes
* @tech-leads @senior-developers

# Domain-specific reviewers
/src/Academia.Domain/ @domain-experts @architects
/src/Academia.Infrastructure/ @infrastructure-team
/src/Academia.API/ @api-team
/tests/ @qa-team @tech-leads

# Critical files require additional approval
/src/Academia.Domain/Aggregates/ @architects @domain-experts
/.github/ @devops-team @tech-leads
/docs/ @technical-writers @tech-leads
```

#### Review Checklist

```markdown
## Code Review Checklist

### Functionality

- [ ] Code changes match the requirements
- [ ] Business logic is correctly implemented
- [ ] Edge cases are handled appropriately
- [ ] Error handling is comprehensive

### Code Quality

- [ ] Code follows established patterns and conventions
- [ ] No code duplication or unnecessary complexity
- [ ] Proper separation of concerns maintained
- [ ] Dependencies are appropriately managed

### Testing

- [ ] Unit tests cover new/changed functionality
- [ ] Integration tests validate critical workflows
- [ ] Test names clearly describe scenarios
- [ ] Tests are independent and deterministic

### Security

- [ ] No hardcoded secrets or sensitive data
- [ ] Input validation implemented where needed
- [ ] Authorization checks appropriate for functionality
- [ ] No SQL injection or XSS vulnerabilities introduced

### Performance

- [ ] No obvious performance issues introduced
- [ ] Database queries are efficient
- [ ] Caching strategies implemented where appropriate
- [ ] Resource cleanup properly handled

### Documentation

- [ ] Public APIs documented
- [ ] Complex business logic commented
- [ ] README updated if necessary
- [ ] Architecture decisions recorded if applicable
```

## Merge Strategies

### Merge Strategy by Branch Type

#### Feature Branches → Develop

- **Strategy**: Squash and merge
- **Rationale**: Maintains clean history, groups related commits

```bash
# Squash merge example
git checkout develop
git merge --squash feature/ACA-123-implement-enrollment
git commit -m "feat(enrollment): implement student course enrollment

Complete implementation of enrollment system including validation,
business rules enforcement, and comprehensive test coverage.

Resolves: ACA-123"
```

#### Hotfix Branches → Main

- **Strategy**: Merge commit (no fast-forward)
- **Rationale**: Preserves hotfix branch history for audit purposes

```bash
# Hotfix merge example
git checkout main
git merge --no-ff hotfix/ACA-789-security-patch
git tag v1.1.1
git push origin main --tags
```

#### Release Branches → Main

- **Strategy**: Merge commit (no fast-forward)
- **Rationale**: Clear release points in history

```bash
# Release merge example
git checkout main
git merge --no-ff release/v1.2.0
git tag v1.2.0
git push origin main --tags

# Merge release changes back to develop
git checkout develop
git merge --no-ff release/v1.2.0
git push origin develop
```

### Conflict Resolution Guidelines

#### Merge Conflict Resolution Process

```bash
# When conflicts occur during merge/rebase
git status  # Identify conflicted files

# Edit conflicted files to resolve conflicts
# Remove conflict markers (<<<<<<<, =======, >>>>>>>)
# Keep appropriate changes from both branches

# Stage resolved files
git add <resolved-files>

# Continue merge/rebase
git rebase --continue  # if rebasing
git commit             # if merging

# Verify resolution
git log --oneline -10
git push origin <branch-name>
```

#### Conflict Resolution Best Practices

1. **Communicate Early**: Notify team of significant conflicts
2. **Test After Resolution**: Run full test suite after conflict resolution
3. **Review Resolution**: Have conflicts reviewed by another team member
4. **Document Decisions**: Record resolution rationale for complex conflicts

## Release Management

### Release Branch Workflow

```mermaid
gitgraph
    commit id: "v1.0.0"
    branch develop
    checkout develop
    commit id: "Feature A"
    commit id: "Feature B"
    branch release/v1.1.0
    checkout release/v1.1.0
    commit id: "Version bump"
    commit id: "Bug fixes"
    checkout main
    merge release/v1.1.0
    commit id: "v1.1.0" tag: "v1.1.0"
    checkout develop
    merge release/v1.1.0
```

### Version Tagging Strategy

```bash
# Semantic versioning: MAJOR.MINOR.PATCH
# MAJOR: Breaking changes
# MINOR: New features (backward compatible)
# PATCH: Bug fixes (backward compatible)

# Create annotated tags for releases
git tag -a v1.2.0 -m "Release version 1.2.0

Features:
- Student enrollment system
- Grade calculation improvements
- Performance optimizations

Bug Fixes:
- Fixed null reference in course search
- Resolved enrollment validation edge case"

# Push tags to remote
git push origin --tags
```

### Hotfix Workflow

```bash
# Create hotfix from main branch
git checkout main
git checkout -b hotfix/v1.1.1-security-patch

# Make minimal changes to fix critical issue
git add .
git commit -m "fix(security): patch critical authentication vulnerability

Apply security patch for authentication bypass vulnerability
discovered in production. This is a minimal change to address
the immediate security concern.

CVE-2024-12345
Resolves: ACA-SECURITY-001"

# Merge to main and tag
git checkout main
git merge --no-ff hotfix/v1.1.1-security-patch
git tag v1.1.1
git push origin main --tags

# Merge to develop to include fix in future releases
git checkout develop
git merge --no-ff hotfix/v1.1.1-security-patch
git push origin develop

# Delete hotfix branch
git branch -d hotfix/v1.1.1-security-patch
git push origin --delete hotfix/v1.1.1-security-patch
```

## Advanced Git Workflows

### Interactive Rebase for History Cleanup

```bash
# Clean up commit history before creating PR
git rebase -i HEAD~3

# Rebase interactive options:
# pick = use commit
# reword = use commit, but edit message
# edit = use commit, but stop for amending
# squash = use commit, but meld into previous commit
# fixup = like squash, but discard commit message
# drop = remove commit
```

### Cherry-picking for Selective Merges

```bash
# Apply specific commit from another branch
git cherry-pick <commit-hash>

# Cherry-pick range of commits
git cherry-pick <start-commit>..<end-commit>

# Cherry-pick with conflict resolution
git cherry-pick <commit-hash>
# Resolve conflicts
git add .
git cherry-pick --continue
```

### Stashing for Context Switching

```bash
# Save work in progress
git stash push -m "WIP: enrollment validation logic"

# List stashes
git stash list

# Apply specific stash
git stash apply stash@{1}

# Pop latest stash (apply and remove)
git stash pop

# Create branch from stash
git stash branch feature/new-work stash@{0}
```

## Repository Maintenance

### Branch Cleanup Automation

```bash
#!/bin/bash
# cleanup-merged-branches.sh

# Delete local branches that have been merged to main
git branch --merged main | grep -v "\* main" | xargs -n 1 git branch -d

# Delete remote tracking branches that no longer exist
git remote prune origin

# List branches not merged to main (for review)
git branch --no-merged main
```

### Repository Statistics and Health

```bash
# Check repository size and largest files
git count-objects -vH

# Find largest files in repository history
git rev-list --objects --all | git cat-file --batch-check='%(objecttype) %(objectname) %(objectsize) %(rest)' | sed -n 's/^blob //p' | sort --numeric-sort --key=2 | tail -10

# Analyze commit activity
git shortlog -sn --since="1 month ago"

# Check for large commits
git log --oneline --since="1 month ago" --stat | grep -E "^\s+\d+\s+files? changed" | head -10
```

## Git Hooks Implementation

### Pre-commit Hook

```bash
#!/bin/sh
# .git/hooks/pre-commit

echo "Running pre-commit checks..."

# Check for merge conflict markers
if grep -r "<<<<<<< " . --exclude-dir=.git; then
    echo "Error: Merge conflict markers found"
    exit 1
fi

# Check commit message format (if amending)
if [ -f .git/COMMIT_EDITMSG ]; then
    commit_msg=$(cat .git/COMMIT_EDITMSG)
    if ! echo "$commit_msg" | grep -qE "^(feat|fix|docs|style|refactor|test|chore|perf|ci)(\(.+\))?: .+"; then
        echo "Error: Commit message must follow conventional commit format"
        echo "Format: type(scope): description"
        exit 1
    fi
fi

# Run tests
echo "Running tests..."
dotnet test --logger:"console;verbosity=minimal"
if [ $? -ne 0 ]; then
    echo "Error: Tests failed"
    exit 1
fi

echo "Pre-commit checks passed"
exit 0
```

### Commit Message Hook

```bash
#!/bin/sh
# .git/hooks/commit-msg

commit_regex='^(feat|fix|docs|style|refactor|test|chore|perf|ci)(\(.+\))?: .{1,50}'

if ! grep -qE "$commit_regex" "$1"; then
    echo "Invalid commit message format!"
    echo "Format: type(scope): description"
    echo "Types: feat, fix, docs, style, refactor, test, chore, perf, ci"
    echo "Example: feat(enrollment): add student course enrollment"
    exit 1
fi
```

## Troubleshooting Common Issues

### Resolving Detached HEAD

```bash
# If you're in detached HEAD state
git status  # Check current state

# Create branch from current position
git checkout -b recovery-branch

# Or return to main branch
git checkout main
```

### Recovering Lost Commits

```bash
# Find lost commits using reflog
git reflog

# Recover specific commit
git checkout <commit-hash>
git checkout -b recovery-branch

# Or cherry-pick lost commit
git cherry-pick <commit-hash>
```

### Undoing Changes

```bash
# Undo last commit (keep changes in working directory)
git reset --soft HEAD~1

# Undo last commit (discard changes)
git reset --hard HEAD~1

# Undo changes to specific file
git checkout HEAD -- <filename>

# Revert commit (create new commit that undoes changes)
git revert <commit-hash>
```

## Related Documentation References

- [SDLC Process Guidelines](./sdlc-process.instructions.md)
- [Code Review Standards](./documentation-guidelines.instructions.md)
- [CI/CD Pipeline Configuration](./deployment-operations.instructions.md)
- [Security Compliance Requirements](./security-compliance.instructions.md)

## Validation Checklist

Before considering the Git workflow implementation complete, verify:

- [ ] Branch naming conventions are documented and enforced
- [ ] Commit message format follows conventional commits standard
- [ ] Pull request template includes all required sections
- [ ] Merge strategies are appropriate for each branch type
- [ ] Automated checks validate code quality before merge
- [ ] Code review requirements include functional and security validation
- [ ] Release management process supports semantic versioning
- [ ] Hotfix workflow provides rapid production issue resolution
- [ ] Repository maintenance procedures prevent repository bloat
- [ ] Git hooks enforce quality standards at commit time
- [ ] Troubleshooting procedures cover common Git scenarios
- [ ] All team members understand and can execute the workflow
- [ ] Workflow integrates with CI/CD pipeline requirements
- [ ] Security considerations are addressed in all workflow stages
