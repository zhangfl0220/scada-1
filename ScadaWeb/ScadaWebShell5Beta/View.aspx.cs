﻿/*
 * Copyright 2016 Mikhail Shiryaev
 * 
 * Licensed under the Apache License, Version 2.0 (the "License");
 * you may not use this file except in compliance with the License.
 * You may obtain a copy of the License at
 * 
 *     http://www.apache.org/licenses/LICENSE-2.0
 * 
 * Unless required by applicable law or agreed to in writing, software
 * distributed under the License is distributed on an "AS IS" BASIS,
 * WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied.
 * See the License for the specific language governing permissions and
 * limitations under the License.
 * 
 * 
 * Product  : Rapid SCADA
 * Module   : SCADA-Web
 * Summary  : View web form
 * 
 * Author   : Mikhail Shiryaev
 * Created  : 2016
 * Modified : 2016
 */

using Scada.UI;
using Scada.Web.Plugins;
using Scada.Web.Shell;
using System;
using System.Text;

namespace Scada.Web
{
    /// <summary>
    /// View web form
    /// <para>Веб-форма представления</para>
    /// </summary>
    public partial class WFrmView : System.Web.UI.Page
    {
        private UserData userData;       // данные пользователя приложения
        protected int initialViewID;     // ид. первоначального представления
        protected string initialViewUrl; // ссылка первоначального представления
        protected string phrases;        // локализованные фразы


        /// <summary>
        /// Генерировать HTML-код нижних закладок
        /// </summary>
        protected string GenerateBottomTabsHtml()
        {
            const string TabTemplate = "<div class='tab' data-code='{0}' data-url='{1}' data-depends='{2}'>{3}</div>";

            StringBuilder sbHtml = new StringBuilder();

            foreach (DataWindowSpec dataWnd in userData.UserContent.DataWindows)
                sbHtml.AppendFormat(TabTemplate, dataWnd.ContentTypeCode, ResolveUrl(dataWnd.Url), 
                    dataWnd.DependsOnView ? "true" : "false", dataWnd.Name);

            return sbHtml.ToString();
        }


        protected void Page_Load(object sender, EventArgs e)
        {
            userData = UserData.GetUserData();
            userData.CheckLoggedOn(true);

            // перевод веб-страницы
            Translator.TranslatePage(Page, "Scada.Web.WFrmView");

            Localization.Dict dict;
            Localization.Dictionaries.TryGetValue("Scada.Web.WFrmView.Js", out dict);
            phrases = WebUtils.DictionaryToJs(dict);

            // получение ид. и ссылки представления для загрузки
            int.TryParse(Request.QueryString["viewID"], out initialViewID);
            ViewNode viewNode;

            if (initialViewID > 0)
            {
                viewNode = userData.UserViews.GetViewNode(initialViewID);
            }
            else
            {
                viewNode = userData.UserViews.GetFirstViewNode();
                initialViewID = viewNode == null ? 0 : viewNode.ViewID;
            }

            initialViewUrl = viewNode == null || string.IsNullOrEmpty(viewNode.ViewUrl) ?
                ResolveUrl(UrlTemplates.NoView) : viewNode.ViewUrl;

            ((MasterMain)Master).SelectedViewID = initialViewID;
        }
    }
}