#!/usr/bin/env bash
#
# Increments the semantic version in Source/Directory.Build.props.
#
# Usage:
#   bash Scripts/bump-version.sh [--dry-run] <major|minor|patch> [props-file]
#
# The new version is written to stdout. With --dry-run the file is left
# untouched and only the version that would be used is printed.

set -euo pipefail

dry_run=false
if [[ "${1:-}" == "--dry-run" ]]; then
  dry_run=true
  shift
fi

part="${1:-patch}"
props_file="${2:-Source/Directory.Build.props}"

# Accept Patch, PATCH, patch, etc.
part="${part,,}"

case "$part" in
  major|minor|patch) ;;
  *)
    echo "error: version part must be one of major, minor or patch (got '$part')" >&2
    exit 2
    ;;
esac

if [[ ! -f "$props_file" ]]; then
  echo "error: file not found: $props_file" >&2
  exit 2
fi

version_count="$(grep -o '<Version>' "$props_file" | wc -l || true)"
if [[ "$version_count" -ne 1 ]]; then
  echo "error: expected exactly one <Version> element in $props_file, found $version_count" >&2
  exit 1
fi

current="$(sed -nE 's:.*<Version>([0-9]+\.[0-9]+\.[0-9]+)</Version>.*:\1:p' "$props_file")"
if [[ -z "$current" ]]; then
  echo "error: no <Version>x.y.z</Version> element found in $props_file" >&2
  exit 1
fi

IFS=. read -r major minor patch <<< "$current"

case "$part" in
  major)
    major=$((major + 1))
    minor=0
    patch=0
    ;;
  minor)
    minor=$((minor + 1))
    patch=0
    ;;
  patch)
    patch=$((patch + 1))
    ;;
esac

new_version="$major.$minor.$patch"

if [[ "$dry_run" == true ]]; then
  echo "$new_version"
  exit 0
fi

sed -i -E "s#(<Version>)[0-9]+\.[0-9]+\.[0-9]+(</Version>)#\1${new_version}\2#" "$props_file"

written="$(sed -nE 's:.*<Version>([0-9]+\.[0-9]+\.[0-9]+)</Version>.*:\1:p' "$props_file")"
if [[ "$written" != "$new_version" ]]; then
  echo "error: failed to update $props_file to $new_version (found '$written')" >&2
  exit 1
fi

echo "$new_version"
