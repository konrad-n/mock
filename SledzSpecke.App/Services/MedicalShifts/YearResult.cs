﻿using System;
using System.Collections.Generic;
using System.IO.Pipelines;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using SledzSpecke.App.Models;
using SledzSpecke.App.Models.Enums;
using SledzSpecke.App.Services.Authentication;
using SledzSpecke.App.Services.Database;
using SledzSpecke.App.Services.Specialization;

namespace SledzSpecke.App.Services.MedicalShifts
{
    public partial class MedicalShiftsService
    {
        private class YearResult
        {
            public int Year { get; set; }
        }
    }
}