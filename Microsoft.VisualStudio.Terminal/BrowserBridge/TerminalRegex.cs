using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Interop;
using System;
using System.IO;
using System.Text.RegularExpressions;

namespace EmbeddedTerminal
{
    internal static class TerminalRegex
    {
        /* Regex's originally from VSCode.
         * VSCode is licensed under the MIT license, original sources and license.txt can be found here:
         * https://github.com/Microsoft/vscode
         */
        const string winDrivePrefix = "[a-zA-Z]:";
        const string winPathPrefix = "(" + winDrivePrefix + "|\\.\\.?|\\~)";
        const string winPathSeparatorClause = "(\\\\|\\/)";
        const string winExcludedPathCharactersClause = "[^\\0<>\\?\\|\\/\\s!$`&*()\\[\\]+\'\":;]";
        /** A regex that matches paths in the form c:\foo, ~\foo, .\foo, ..\foo, foo\bar */
        const string winLocalLinkClause = "((" + winPathPrefix + "|(" + winExcludedPathCharactersClause + ")+)?(" + winPathSeparatorClause + "(" + winExcludedPathCharactersClause + ")+)+)";

        /** As xterm reads from DOM, space in that case is nonbreaking char ASCII code - 160,
        replacing space with nonBreakningSpace or space ASCII code - 32. */
        private static string lineAndColumnClause = String.Join("|", new string[] {
            "((\\S*) on line ((\\d+)(, column (\\d+))?))", // (file path) on line 8, column 13
            "((\\S*):line ((\\d+)(, column (\\d+))?))", // (file path):line 8, column 13
            "(([^\\s\\(\\)]*)(\\s?[\\(\\[](\\d+)(,\\s?(\\d+))?)[\\)\\]])", // (file path)(45), (file path) (45), (file path)(45,18), (file path) (45,18), (file path)(45, 18), (file path) (45, 18), also with []
            "(([^:\\s\\(\\)<>\'\"\\[\\]]*)(:(\\d+))?(:(\\d+))?)" // (file path):336, (file path):336:9
        }).Replace(" ", "[" + "\u00A0" + "]");

        private static Regex localLinkPattern = new Regex(winLocalLinkClause + "(" + lineAndColumnClause + ")");
        // Changing any regex may effect this value, hence changes this as well if required.
        const int winLineAndColumnMatchIndex = 12;
        const int unixLineAndColumnMatchIndex = 15;

        // Each line and column clause have 6 groups (ie no. of expressions in round brackets)
        const int lineAndColumnClauseGroupCount = 6;

        public static Regex LocalLinkRegex => localLinkPattern;

        public static bool ValidateLocalLink(string link)
        {
            var url = ExtractLinkUrl(link);
            if (url == null)
            {
                return false;
            }

            var path = PreprocessPath(url);
            if (link == null)
            {
                return false;
            }

            return File.Exists(path);
        }

        public static void HandleLocalLink(string uri)
        {
            var url = ExtractLinkUrl(uri);
            var path = PreprocessPath(url);

            var lcinfo = ExtractLineColumnInfo(uri);


            VsShellUtilities.OpenDocument(ServiceProvider.GlobalProvider, path, Guid.Empty, out _, out _, out _, out var textView);
            if (textView != null)
            {
                // Indexing in an IVsTextView is zero-based, whereas values returned from a build should be one-based.
                textView.SetCaretPos(lcinfo.Item1 - 1, lcinfo.Item2 - 1);
                textView.CenterLines(lcinfo.Item1 - 1, 1);

                textView.SendExplicitFocus();
            }
        }

        private static string PreprocessPath(string path)
        {
            ThreadHelper.ThrowIfNotOnUIThread();
            // resolve ~
            if (path[0] == '~')
            {
                var homeDrive = Environment.GetEnvironmentVariable("HOMEDRIVE");
                var homePath = Environment.GetEnvironmentVariable("HOMEPATH");

                if (homeDrive == null || homePath == null)
                {
                    return null;
                }
                path = homeDrive + "\\" + homePath + path.Substring(1);
            }

            // resolve relative links
            var regex = new Regex("^[a-zA-Z]:");
            if (!regex.IsMatch(path))
            {
                IVsSolution solutionService = (IVsSolution)Package.GetGlobalService(typeof(SVsSolution));
                solutionService.GetSolutionInfo(out string solutionDir, out _, out _);
                if (solutionDir == null)
                {
                    return null;
                }
                path = Path.Combine(solutionDir, path);
            }

            return path;
        }

        private static string ExtractLinkUrl(string link)
        {
            var groups = localLinkPattern.Match(link).Groups;

            if (groups.Count == 0)
            {
                return null;
            }
            return groups[1].ToString();
        }

        private static Tuple<int, int> ExtractLineColumnInfo(string link)
        {
            var row = 0;
            var col = 0;
            var groups = localLinkPattern.Match(link).Groups;

            for (var i = 0; i < lineAndColumnClause.Length; i++)
            {
                var lineMatchIndex = winLineAndColumnMatchIndex + (lineAndColumnClauseGroupCount * i);
                var rowNumber = groups[lineMatchIndex].ToString();

                if (rowNumber != "")
                {
                    row = int.Parse(rowNumber);

                    var colNumber = groups[lineMatchIndex + 2].ToString();
                    if (colNumber != "")
                    {
                        col = int.Parse(colNumber);
                    }
                    break;
                }
            }

            return new Tuple<int, int>(row, col);
        }
    }
}
