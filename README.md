# Daily Transaction Digest Email

Azure Function that uses Plaid api to send an email every morning with the previous days transactions.

App settings to set:
 - SendGridApiKey
 - plaid_client_id
 - plaid_secret
 - accounts    (json blob)
 - report_email