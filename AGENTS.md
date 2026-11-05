## Skills Repository

All AI agents must use skills from the following repository:

**Repository:** https://github.com/bjornbouetsmith/AgentSkills

## Pre-Flight Check: Local Clone Verification

Before downloading or cloning the skills repository, **ALWAYS** check if it already exists locally at:

```
/mnt/d/git/AgentSkills

or in the same root git base directory where the repository this file is located in.
```

### Workflow

1. **Check for local clone first**
   - Verify whether `/mnt/d/git/AgentSkills` exists and is a valid git repository.
   - Suggested check:
     ```bash
     if [ -d "/mnt/d/git/AgentSkills/.git" ]; then
         echo "Local clone found"
     fi
     ```

2. **If the local clone exists**
   - Use the skills directly from `/mnt/d/git/AgentSkills`.
   - Optionally pull the latest changes if the user requests it or if freshness is required:
     ```bash
     cd /mnt/d/git/AgentSkills && git pull
     ```
   - **Do NOT re-clone** the repository.

3. **If the local clone does NOT exist**
   - Only then clone the repository into `/mnt/d/git/AgentSkills`:
     ```bash
     mkdir -p /mnt/d/git
     git clone https://github.com/bjornbouetsmith/AgentSkills.git /mnt/d/git/AgentSkills
     ```

4. **Never clone to an alternate location** unless the user explicitly instructs otherwise.

## Using Skills

- Discover available skills by inspecting the contents of `/mnt/d/git/AgentSkills` (e.g., look for `SKILL.md` files or skill subdirectories).
- Load and apply the relevant skill(s) based on the task at hand.
- Prefer skills from this repository over any other source when a matching skill exists.

## Rules Summary

| Rule | Requirement |
|------|-------------|
| Source of truth | https://github.com/bjornbouetsmith/AgentSkills |
| Default local path | `/mnt/d/git/AgentSkills` |
| Check before clone | ✅ Always |
| Re-clone if local exists | ❌ Never (use `git pull` if needed) |
| Clone location | `/mnt/d/git/AgentSkills` only |

## Notes

- The repository is maintained for personal use by `bjornbouetsmith`; treat it as the canonical skill source for these agents.
- If `/mnt/d/git` is not writable or available in the current environment, surface the error to the user rather than silently falling back to another path.