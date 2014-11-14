Keyboard Layout of Total Commander
==================================

Key                    Action
~~~                    ~~~~~~

F1                     Help
F2                     Reread source window
F3                     List files
F4                     Edit files
F5                     Copy files
F6                     Rename files
F7                     Create directory
F8 or DEL              Delete files to recycle bin /delete directly - according to configuration
F9                     Activate menu above source window (left or right)
F10                    Activate left menu or leave menu
ALT+F1                 Change left drive
ALT+F2                 Change right drive
ALT+F3                 Use external viewer
ALT+SHIFT+F3           Start Lister and load file with internal viewer (no plugins or multimedia)
ALT+F4                 Exit | Minimize (with option MinimizeOnClose, see help)
ALT+F5                 Pack selected files
ALT+SHIFT+F5           Move to archive
ALT+F6                 Unpack specified files from archive under cursor, or selected archives (use Alt+F9 on Windows 95)
ALT+F7                 Find
ALT+SHIFT+F7           Find in separate process
ALT+F8                 Opens the history list of the command line
ALT+F9                 Same as ALT+F6 (because ALT+F6 is broken on Windows 95)
ALT+SHIFT+F9           Test archives for integrity
ALT+F10                Open a directory tree window
ALT+F11                Opens left current directory bar (breadcrumb bar)
ALT+F12                Opens right current directory bar (breadcrumb bar)
ALT+SHIFT+F11          Focus the button bar to use it with the keyboard
ALT+left or right      Open previous/next directory in list of already visited dirs
ALT+down               Open list of already visited dirs
SHIFT+F1               Choose custom columns view
SHIFT+F2               Compare file lists
SHIFT+F3               List only file under cursor, when multiple files selected
SHIFT+F4               Create new text file and load into editor
SHIFT+F5               Copy files (with rename) in the same directory
SHIFT+CTRL+F5          Create shortcut of files in target directory
SHIFT+F6               Rename files in the same directory
SHIFT+F8/DEL           Delete directly / delete to recycle bin - according to configuration
SHIFT+F10              Show context menu
SHIFT+ESC              Iconize Wincmd
NUM +                  Expand selection
NUM -                  Shrink selection
NUM *                  Invert selection
NUM /                  Restore selection
SHIFT+NUM +            Like NUM +, but files and folders if NUM + selects just files (and vice versa)
SHIFT+NUM -            Always removes the selection just from files (NUM- from files and folders)
SHIFT+NUM *            Like NUM *, but files and folders if NUM * inverts selection of just files (and vice versa)
CTRL+NUM +             Select all
CTRL+SHIFT+NUM +       Select all (files and folders if CTRL+NUM + selects only files)
CTRL+NUM -             Deselect all
CTRL+SHIFT+NUM -       Deselect all (always files, no folders)
ALT+NUM +              Select all files with the same extension
ALT+NUM -              Remove selection from files with the same extension
BACKSPACE or CTRL+PgUp Change to parent directory (corresponds to cd ..)
CTRL+PgDn              Open directory/archive (also self extracting .EXE archives)
CTRL+<                 Jump to the root directory (most European keyboards)
CTRL+\                 Jump to the root directory (US keyboard)
CTRL+left or right     Open directory/archive and display it in the target window.
                       If the cursor is not on a directory name, the current directory is
                       displayed instead.
CTRL+F1                File display 'brief' (only file names)
CTRL+SHIFT+F1          Thumbnails view (preview images)
CTRL+F2                File display 'full' (all file details)
CTRL+SHIFT+F2          Comments view
CTRL+F3                Sort by name
CTRL+F4                Sort by extension
CTRL+F5                Sort by date/time
CTRL+F6                Sort by size
CTRL+F7                Unsorted
CTRL+F8                Display directory tree
CTRL+SHIFT+F8          Cycle through separate directory tree states: one tree, two trees, off
CTRL+F9                Print file under cursor using the associated program
CTRL+F10               Show all files
CTRL+F11               Show only programs
CTRL+F12               Show user defined files
TAB                    Switch between left and right file list
SHIFT+TAB              Switch between current file list and separate tree
Letter                 Redirected to command line, cursor jumps to command line
INSERT                 Select file or directory
SPACE                  Select file or directory (as INSERT). If SPACE is used on an unselected
                       directory under the cursor, the contents in this directory are counted
                       and the size is shown in the 'full' view instead of the string <DIR>.
ENTER                  Change directory/run program/run associated program/execute command line
                       if not empty. If the source directory shows the contents of an archive,
                       further information on the packed file is given.
SHIFT+ENTER            1. Run command line/program under cursor via Windows shell and
                       leave open the program's window. Only works if NOCLOSE.EXE is present in your
                       Totalcmd directory!
                       2. With ZIP files: use alternative choice of these (as chosen in Packer
                       config): (Treat archives like directories <-> call associated program,
                       i.e. winzip or quinzip).
ALT+SHIFT+ENTER        The contents of all directories in the current directory are counted.
                       The sizes of the directories are then shown in the 'full' view instead
                       of the string <DIR>.
ALT+ENTER              Show property sheet.
CTRL+A                 Select all
CTRL+B                 Directory branch: show contents of current dir and all subdirs in one list
CTRL+SHIFT+B           Directory branch for selected directories: show contents of selected dir
                       and all subdirs in one list
CTRL+C                 Copy files to clipboard
CTRL+D                 Open directory hotlist ('bookmarks')
CTRL+F                 Connect to FTP server
CTRL+SHIFT+F           Disconnect from FTP server, port connection, and some file system plugins
CTRL+I                 Switch to target directory
CTRL+L                 Calculate occupied space (of the selected files)
CTRL+M                 Multi-Rename-Tool
CTRL+SHIFT+M           Change FTP transfer mode
CTRL+N                 New FTP connection (enter URL or host address)
CTRL+P                 Copy current path to command line
CTRL+Q                 Show Quick View panel instead of a file list
CTRL+R                 Reread source directory
CTRL+S                 Open Quick Filter dialog and activate filter
CTRL+SHIFT+S           Open Quick Filter dialog and reactivate last-used filter
CTRL+T                 Open new folder tab and activate it
CTRL+SHIFT+T           Open new folder tab, but do not activate it
CTRL+U                 Exchange directories
CTRL+SHIFT+U           Exchange directories and tabs
CTRL+V                 Paste from clipboard to current dir
CTRL+W                 Close currently active tab
CTRL+SHIFT+W           Close all open tabs
CTRL+X                 Cut files to clipboard
CTRL+Z                 Edit comment
CTRL+UP                Open dir under cursor in new tab
CTRL+SHIFT+UP          Open dir under cursor in other window (new tab)
CTRL+TAB               Jump to next tab
CTRL+SHIFT+TAB         Jump to previous tab
ALTGR+Letter(s) or     Quick search for file name (starting with specified letters) in current
CTRL+ALT+Letter(s)     directory (Support hotkeys Ctrl+X, Ctrl+C, Ctrl+V and Ctrl+A; use
                       Ctrl+S for search filter on/off)

Command Line: Keys
==================

If Total Commander is active, nearly all keyboard input is directed to the command line.
Here the layout for some keys:

ENTER                  The command line is executed, if it contains at least one character
                       (otherwise, the program under the cursor in the source window is started).
                       If the command is cd, md or rd, it is executed internally. If it is an
                       internal DOS command, DOS will be executed with this command. Otherwise,
                       a program with the given name is executed.
SHIFT-ENTER            Like ENTER, but via Windows shell. After the called DOS program
                       is terminated, its window is not automatically closed. This will only
                       work if the file NOCLOSE.EXE is in your Totalcmd directory.
CTRL-ENTER             The file under the cursor in the source directory is appended to the
                       command line.
CTRL-SHIFT-ENTER       The file under the cursor including its path is appended to the command
                       line.
CTRL-CURSORDOWN or     A list with the latest command lines (history-list) is opened. Hold down
ALT+F8                 the CTRL key and use the up and down cursor keys to select an entry. By
                       pressing the left or right key, you can edit the command line. This
                       automatically closes the list.
TAB or Cursor Keys     Puts the cursor back to the source directory. Cursor left and cursor
                       right move the cursor through the command line, even if the cursor was
                       in the source directory before. If brief was selected, these keys move
                       the cursor through the source directory. You then have to hold down
                       SHIFT to move it inside the command line. Cursor up and cursor down
                       move the cursor bar inside the source window, even if it was inside
                       the command line.
ESC                    Clears the command line, put cursor back in file window.
CTRL+A                 Select entire command line
CTRL+Y                 Clears the command line, cursor stays in command line.
CTRL+C                 Copy selected text to clipboard
CTRL+X                 Cut selected text to clipboard
CTRL+V                 Paste text from clipboard to command line.
CTRL+E                 Copy previous command to command line
CTRL+K                 Delete to end of line
CTRL+W                 Delete word to the left of the cursor
CTRL+T                 Delete word to the right of the cursor
