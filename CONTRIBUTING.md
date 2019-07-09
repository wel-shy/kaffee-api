# Contributing

## Committing

Git commits should be made using [Gitmoji](https://gitmoji.carloscuesta.me/)
where commits are prefixed with an emoji, related to the content of the
commit for improved visual grepping of commit type.

You can install and configure the gitmoji packaged with the command below.
Make sure to select the emoji code commit style `:sparkles:` rather than the
unicode version `✨`.

```console
npm i -g gitmoji-cli && gitmoji --config
```

And commit with:

```console
gitmoji -c
```

Commits can also be made with the npm packaged `conventional-committer`to
create a format of commit messages that can be used to auto generate
changelogs.

This can be installed with:

```console
npm i -g conventional-committer
```

## Branching

This project uses the [Git Flow](https://www.atlassian.com/git/tutorials/comparing-workflows/gitflow-workflow) model.

## Updating into Master

Before merging from dev into master, you should create a new branch called
`realease/x.x.x`, where `x.x.x` is the new version number. This branch should be
properly tested, and all instances of the version number should be updated
and documentation should be updated. A pull request can be made to merge
this branch into master. All tests on the `release` branch should pass before
a request is made.

Merges into master should be made via a pull request. This should make it
easier to identify when and who introduced and approved new features or
changes. Pull requests also ensure code is inline with the rest of the
project when reviewed before approval. On a successful merge the changelog
should be updated with a description of the change (see below).

After the `release/x.x.x` branch has been merged, the master branch should be
tagged with the new version number in the format `v-x.x.x`. The `release/x.x.x`
should then be merged into `dev` and then the `release/x.x.x` branch should
be deleted.

## Hotfixing

Hot fixes should be made by branching `master`, and creating a new branch
called `hotfix/x.x.x+1` where the fix version is updated. Changes should be
made here, and when completed a pull request into `master` should be made.
Once merged into `master`, `master` should be tagged with the new version
number. `hotfix/x.x.x+1` should then be merged into `dev`, and the hotfix
branch then deleted. The contributing guide should be updated (see below).
