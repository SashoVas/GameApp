﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GameApp.Services.Contracts
{
    public interface IReviewService
    {
        Task<bool> Rate(string gameName, int points, string userId);
    }
}
