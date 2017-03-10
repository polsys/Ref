# Manual tests for Ref

(At least) These tests must be run before any release and should be run after major changes. I suggest going through the tests in order.

### Basics

- [ ] Open the `ManualTests.refproject` file.
- [ ] For each entry:
  - Select the entry and make sure it seems correct.
  - Open the Copy Reference dialog and check the result with all the citation/output styles.
  - Select each note and make sure it seems correct.
  - Make sure that the Copy Reference dialog opens when a note is selected.

### Export
  
- [ ] Export the catalogue as BibTeX file `test.bib` and make sure that the `bibtex.tex` project builds correctly.
- [ ] Export the catalogue as a Word file. Import each entry in Word, add a bibliography to the document and make sure it is correct.

### Editing
- [ ] Add a new entry of each type.
- [ ] Add a note to at least two entries of different types.
- [ ] Modify the title of an existing entry and check that the change is reflected in the list.
- [ ] Start editing an entry, and select another entry. Cancel the edit when asked. Make sure that the original entry has not been modified.
- [ ] Do the same, but save the changes this time. 
- [ ] Save the catalogue as a new file. Check the window title.
- [ ] Remove all the entries but one. Try both removing a note and removing an entry with attached notes.
- [ ] Create a new project. Do not save the changes. Check the window title.
- [ ] Save the project. Make sure that you are asked a file name. Check that the saved project file does not contain any entries. 