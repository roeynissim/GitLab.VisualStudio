﻿using Microsoft.TeamFoundation.Controls;

namespace CodeCloud.TeamFoundation
{

    static class NavigationItemPriority
    {
        public const int PullRequests = TeamExplorerNavigationItemPriority.GitCommits - 1;
        public const int Wiki = TeamExplorerNavigationItemPriority.Settings - 1;
        public const int Pulse = Wiki - 3;
        public const int Graphs = Wiki - 2;
        public const int Issues = Wiki - 1;
    }
}