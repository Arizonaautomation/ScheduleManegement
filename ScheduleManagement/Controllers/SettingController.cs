﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using TrainningManagement.SessionValidate;

namespace TrainningManagement.Controllers
{
    #region
    [ActionAuthorize]
    #endregion

    public class SettingController : Controller
    {
        // GET: Setting
        public ActionResult PasswordSetting()
        {
            return View();
        }
    }
}