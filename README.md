# Seo.Prerender

Detect search engine crawler then render snapshot. It prevent Ajax content not indexed issue.

## Install

1. Add Seo.Prerender dll reference
1. Add SeoHttpModule in Web Config

    ```xml
    <system.webServer>
      <modules>
        <remove name="SeoModule" />
        <add name="SeoModule" type="Seo.Prerender.SeoHttpModule" />
      </modules>
    </system.webServer>
    ```

1. Add `<meta name="fragment" content="!">` in every pages
1. Add Prerender.config in project root

    ```xml
    <?xml version="1.0" encoding="UTF-8"?>
    <PreRenderSetting xmlns:xsd="http://www.w3.org/2001/XMLSchema" xmlns:xsi="http://www.w3.org/2001/XMLSchema-instance">
      <CrawlerUserAgents>188bot,188crawler</CrawlerUserAgents>
      <DomainSettings>
        <DomainSetting Domain="default" AccessPath="C:\Agilebet\share\snapshot" ByPassPaths="/"/>
        <DomainSetting Domain="uk.ngstar.sb.com" AccessPath="C:\Agilebet\share\uksnapshot" ByPassPaths="/"/>
      </DomainSettings>
      <MailNotifySetting Enable="false" MailServer="smtp.xuenn.com" Port="25" Recipients="andy.chen@xuenn.com,kid.liu@xuenn.com,joseph.tsai@xuenn.com" Sender="srv.cshhelp@xuenn.com" />
    </PreRenderSetting>
    ```

## How to know if it works

1. Add `?_escaped_fragment_` after request url or modify your useragent to search-engine crawler by [chrome-extension](https://chrome.google.com/webstore/detail/user-agent-switcher-for-g/ffhkkpnppgnfaobgihpdblnhmmbodake)
1. Check `developer tool - Network - doc` to see if HTML document in response is already rendered with AJAX content or not
