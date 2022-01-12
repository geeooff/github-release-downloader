# GitHub Release Downloader

This program has been developed to download the latest release of a GitHub public repository.

My use case was beeing able to download Farming Simulator mods when these mods are not yet available in FS's ModHub.

I usually schedule a task in Windows to automatically call this program with the right arguments to keep my mods up to date.

## Command line

```
Usage:
  github-release-downloader [options]

Options:
  --owner <owner> (REQUIRED)    Owner Name
  --repo <repo> (REQUIRED)      Repository Name
  --output <output> (REQUIRED)  Release asset destination file
  --force                       Force download even if output is up to date [default: False]
  --pre-release                 Allow Pre-Release asset downloads [default: False]
  --create-backup               Create a backup of any existing file [default: True]
  --version                     Show version information
  -?, -h, --help                Show help and usage information
```

### Options

- `<owner>` is the repository owner;
  eg. `foo` in `github.com/foo/bar`
- `<repo>` is the repository name;
  eg. `bar` in `github.com/foo/bar`
- `<output>` is the final asset file name on your storage;
  eg. `C:\Users\geoff\Desktop\myfile.zip`.
  Please note `myfile.zip` must match the name of a single asset in the release.
- `--force` will enforce file overwrite even if the destination file is up to date.
- `--create-backup` will create a backup to the file before overwriting it;
  eg. `C:\Users\geoff\Desktop\myfile.zip.bak`
- `--version`


## Limitations

To be able to download an asset from a release, its file name must be known before calling.
