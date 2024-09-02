# WindowsHandlerCheckerWeb
## English
A MVC website to check Handles of existing windows and try to list Handles of its sub elements

Display Handle and Title of all current existing window in Windows. Fetch one of them (using copy&paste) to backend, and try to get the Handle of its elements if it's a regular app window, or Chrome.

It's a play-around project to go through the process of fetching information of windows app. I set it up using my old Win 8.1 laptop I brought in my college, so somehow the framework and structure is out-of-date.

The windowsUtils class is the key file of all the windows related operations. Archive it for need.

## 中文
本项目用于熟悉在windows环境中用C#获取并对可视窗口进行操作。

用的是我用了10年的本科笔记本，才装的Win8，于是乎只能用vs2015建.net 4.5的项目了，悲伤
界面没有美化，主要功能在windowsUtils.cs中，原本是试图做一个自动下载迅雷的网站建winnas用的，但是貌似win8.1上的迅雷是用chrome legacy window包了一层，导致无论是用findwindowex还是自带的分析chrome的库都没法获取窗口内部的组件

反正先传上来存着，以后要做类似的对窗口操作省得到处查了

附注：AI查代码真好用
