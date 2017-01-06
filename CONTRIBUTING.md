# Contributing to Ref

Thank you for your interest in Ref! While this is mostly a personal side project, any contributions are welcome! Please take a minute or two to read my thoughts below:

## Issues and discussion
* As an obligatory reminder, we are human beings. We have our differences, but we can work together. This means being nice.
* If you think you're inexperienced - so am I. It doesn't make you any less valuable contributor.
* When reporting a bug, please try to make a minimal repro. That means
  * finding the least amount of steps to make the bug occur,
  * writing out each step,
  * especially copying any error message completely,
  * detailing what you expected to happen,
  * possibly attaching a screen shot or the saved project file,
  * specifying your Ref version and preferably other environment details.
* When reporting (or implementing) a feature request, please make it as detailed and thought out as possible.
  * What will be the user flow? How will the feature be used?
  * Will the feature conflict with some other feature?
  * Is it practical for Ref? The aim is to make a simple tool, not a multipurpose turbocharged kitchen sink.
  * Are you able to implement the feature, even partially?

## Coding guidelines
* All code should conform to [C# Coding Conventions](https://msdn.microsoft.com/en-us/library/ff926074.aspx).
* Channeling [The Zen of Python](https://www.python.org/dev/peps/pep-0020/) is encouraged.
* Know, or at least acknowledge the existence of, the [*bilities/taxes](https://blogs.msdn.microsoft.com/larryosterman/2006/09/29/bilities/).
  * At the moment, Ref has hard-coded strings, no accessibility support, etc. They are not yet blockers, but will be fixed one day.
* All non-trivial features should have unit tests - Test Driven Development really helps.
  * The UI code is mostly excluded from tests.
  * Failing tests or breaking file changes = no merge. There may be rare exceptions to this rule. (The file format may be changed some day.)
* The project is somewhat MVVM-ish, though not strictly.
* Unit tests are a good place to promote good (popular) science reads.

Thank you for your time! :heart:
