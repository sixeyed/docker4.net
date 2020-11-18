# td-agent is the official distribution of Fluentd with all dependencies
FROM mcr.microsoft.com/windows/servercore:ltsc2019
SHELL ["powershell", "-Command", "$ErrorActionPreference = 'Stop'; $ProgressPreference = 'SilentlyContinue';"]

WORKDIR /tmp
RUN curl -o td-agent.msi http://packages.treasuredata.com.s3.amazonaws.com/4/windows/td-agent-4.0.1-x64.msi; \
    Start-Process msiexec.exe -ArgumentList '/i', 'td-agent.msi', '/quiet', '/norestart' -NoNewWindow -Wait; \
    rm -force td-agent.msi

WORKDIR /opt/td-agent/bin
RUN ./fluent-gem install fluent-plugin-elasticsearch

COPY fluent.conf /fluentd/etc/fluent.conf
ENV FLUENTD_CONF="fluent.conf"
EXPOSE 24224 5140

ENTRYPOINT ["cmd", "/k", "fluentd", "-c", "C:\\fluentd\\etc\\%FLUENTD_CONF%"]
