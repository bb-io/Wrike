# Blackbird.io Wrike — technical documentation

Version **1.0.4** · API v4 · .NET 8 · Build date: 16 November 2026

<!-- begin docs -->

Automate task management and collaboration across Wrike spaces. This release contains **44 actions**, **3 webhook events**, **15 dynamic dropdowns** and two fixed-choice catalogues. It is a build-validated first release, not a claim of production readiness.

## Scope

Included: task search/get/create/update/delete; subtask creation; custom-field value updates; attachments; comments; timelogs; approvals; request-form discovery, submission and prefilling; user lookup/activation status; group search/create/update/delete; shared task bulk updates; asynchronous creation batches; advanced API requests.

Spaces scope task/folder/form/custom-field searches and webhook subscriptions. Workflows and statuses offer space-aware suggestions. For API operations that address a resource directly by ID, the supplied ID is authoritative: a Space ID is a discovery context, not an additional authorization boundary. Selecting a space does not grant access or prove that an independently mapped ID belongs to it. If a folder is supplied to Search tasks, its scope takes precedence over Space ID.

Excluded by agreement: native dashboard/report management and visual proofing annotations. Approval workflows and file attachments are included, but do not implement visual proofing. BI Export is not included. Request-form design/editing, workflow-definition administration, custom-field-definition creation and invitation management do not have dedicated actions in this release; documented endpoints remain reachable through Send request when the connection has their scopes. The app does not promise every Wrike operation through a guided action.

## Connection setup

1. Register an OAuth application in the [Wrike App Console](https://www.wrike.com/appconsole.htm).
2. Register the exact authorization-code callback URL for the target Blackbird environment. The app uses `InvocationContext.UriInfo.AuthorizationCodeRedirectUri` directly, not a hard-coded URL or an implicit-grant callback. Obtain the environment's callback URL from Blackbird before configuring Wrike.
3. Install this package, open Wrike in Blackbird and add a connection.
4. Supply Client ID, Client Secret, Wrike host and the Permissions profile.
5. Authorize access. The validator calls `GET /contacts?me=true` and reports `Connected as <first name> <last name>` on success.

| Field | Control | Behavior |
|---|---|---|
| Client ID | Text | Your registered OAuth application's ID; no embedded credentials |
| Client Secret | Sensitive text | Stored in the connection; also used to derive a separate webhook-signing secret |
| Wrike host | Maintained fixed catalogue | `www.wrike.com`, `app-eu.wrike.com`, `app-us2.wrike.com`; must match the OAuth token response |
| Permissions | Maintained fixed catalogue | Work management and administration (default/recommended), or Work management; blank/omitted profile uses the administration profile |

Authorization and initial token exchange use `login.wrike.com`. API requests use `https://<validated host>/api/v4/` with `Authorization: bearer <token>`. Refresh uses the data-center-specific token URL described in Wrike's OAuth guidance. Expiration uses a two-minute refresh margin. Each successful refresh retains the newly returned refresh token and access token. Token exchange is not automatically retried: an interrupted rotating refresh can require reconnection.

Wrike's OAuth guide has an inconsistency: its refresh prose explicitly requires the data-center host while its example still shows `login.wrike.com`. This implementation follows the explicit regional-host instruction. Regional refresh must be verified live before release approval. OAuth state is passed through the SDK; callback/state verification remains the platform's responsibility.

Disconnecting the Blackbird connection does not remotely revoke Wrike access: no undocumented revocation endpoint is called. Revoke access in Wrike's application controls when needed. Changing the client secret requires recreating affected webhook subscriptions.

## OAuth scope profiles and reconnection

| Permissions profile | Exact requested scopes | Intended use |
|---|---|---|
| Work management and administration (default) | `wsReadWrite,amReadWriteUser,amReadWriteGroup` | Full app coverage including group discovery/creation and user administration |
| Work management | `wsReadWrite` | Task/content workflows with narrower access; group administration requires the other profile |

The administration choice is listed first and marked default. SDK 2.16.0 has no ConnectionProperty default-value member: a blank or omitted profile uses administration scopes in the authorization request, but automatic UI preselection is not promised. An explicitly saved `work` profile is preserved. Upgrading does not expand an existing grant. To add scopes, choose the administration profile and complete OAuth again; ensure the Bird uses that connection. A successful current-user connection test does not prove group-administration access. Wrike roles, plan entitlements and organization policy still apply.

For EU-1, the callback observed during this connection flow is `https://eu-1.blackbird.io/api-rest/connections/AuthorizationCode`. Register that exact URL in Wrike for EU-1, including capitalization and without a trailing slash. Other Blackbird environments supply their own callback. This connector does not use `https://bridge.blackbird.io/api/AuthorizationCode`.

Refresh tokens are internal OAuth credentials, never user inputs. A missing saved refresh token now prompts a new OAuth connection rather than asking users to fill in a token. Incomplete token-exchange results are rejected before returning credentials for storage. The reported live missing-token cause remains unverified; this release does not claim to repair Blackbird vault persistence. HTTP error responses now retain the provider status/message instead of being mislabeled as connectivity failures.

## Task dropdown behavior (1.0.4)

Update task keeps Task ID required and Space ID optional. All inputs using TaskHandler now search the authenticated account's `/tasks` endpoint when no parent scope is selected. A selected space uses `/spaces/{spaceId}/tasks`; a folder, on actions exposing one, takes precedence and uses `/folders/{folderId}/tasks`. Update task does not gain a new Folder ID input. Scope selectors narrow suggestions; mapped task IDs remain authoritative and Wrike enforces access.

Suggestions display title plus ID and store the original API ID. Search text is a trimmed native title-contains filter; blank text omits the title filter. Subtasks are included. Pagination retains all filters; `descendants` is sent only for folder/space routes, as documented by Wrike. Results show at most 100 suggestions after a bounded search of up to 10,000 tasks. Refine the search or select a space if the fetch bound is exceeded. Errors are surfaced, not converted to empty results. No matches returns an empty collection. Typing an ID in the name-search box is not an ID lookup; use manual/mapped values for IDs.

No action names, input names/types/requiredness, credential keys or output bindings change. No new scope or OAuth reauthorization is needed solely for this change. API reference: https://developers.wrike.com/reference/gettasksempty.

## Folder dropdown behavior (1.0.4)

Create task keeps Folder ID required and Space ID optional. Folder suggestions now use the authenticated account's `/folders` endpoint when Space ID is blank; selecting a space restricts the request to `/spaces/{spaceId}/folders`. This applies to every input using FolderHandler. No parent is selected automatically and no existing input is made required.

Suggestions display folder title plus ID and store the original API ID. Search text is a trimmed native title-contains filter; blank text lists the folder tree without unsupported paging parameters. Filtered searches follow pagination and retain the title filter across pages. At most 100 suggestions are displayed, after fetching within the 10,000-item bound. Refine the name or select a space for a narrower catalogue; an exceeded fetch bound or permission failure surfaces an error, not an empty success. An actual no-match response returns empty choices. Manual/mapped IDs remain supported; typing an ID into the name-search box is not an ID lookup. Selecting a suggestion does not grant write permission. Unfiltered results retain Wrike's tree semantics, including projects and special/root nodes.

## Search folders behavior

- `SearchTerm` / **Search term** is optional scalar text (manual or mapped). Leading/trailing whitespace is trimmed; blank means no title filter. A nonblank value is sent as Wrike's native `title` contains filter. It is one phrase, not multiple comma-separated terms, regex or Boolean syntax. Case behavior is Wrike's; no stronger guarantee is made.
- Existing optional scalar `SpaceId` and its searchable dropdown are preserved. Selected: search that space. Blank: use the authenticated account's `/folders` endpoint, subject to access rights. No unrelated discovery inputs change.
- Filtered searches follow `nextPageToken`, retaining `title` on every request. Blank searches use Wrike's folder-tree mode without unsupported paging parameters. Existing unfiltered tree results can include projects and special/root nodes; no new project-only/folder-only filter is imposed.
- Results retain the existing `Folders` output and ID types. No matches returns an empty collection. More than 10,000 results or a remaining page at that limit fails explicitly rather than silently returning an incomplete search.
- API source: https://developers.wrike.com/reference/getspacessinglefolders and https://developers.wrike.com/reference/getfoldersempty.

## Permissions and prerequisites

| Feature | OAuth scopes requested/needed | Additional requirements |
|---|---|---|
| Work-management actions and dropdowns | `wsReadWrite` | Accessible account/resources and the relevant Wrike feature entitlement |
| User administration | `amReadWriteUser` | Administrative rights; activation changes are explicit |
| Group administration | `amReadWriteGroup` | Applicable account-management rights |
| Send request | Only the connection's existing grant | Extra endpoint scopes are not silently granted; unsupported scopes require a separately configured connection/app change |
| Webhooks | Work-management grant | Public HTTPS callback, accessible selected space and working signature handshake |

The administrative profile adds the administrative scopes to `wsReadWrite`; ordinary users can use the work profile without granting administrative scopes. Group discovery may fail with the work profile; this is a permission error, not an empty catalogue. API availability is subject to the account plan: OAuth consent does not enable approvals, custom fields, request forms or time tracking on an ineligible account. All implemented operations are authenticated; no public unauthenticated API mode is exposed. The basic integration requires Wrike app registration and user consent, not an assumed marketplace approval. Organization security policies may impose additional approval requirements.

## Events

| Event | Provider event | Subscription |
|---|---|---|
| On task created | `TaskCreated` | Selected space, recursive |
| On task status changed | `TaskStatusChanged` | Selected space, recursive |
| On task custom field changed | `TaskCustomFieldChanged` | Selected space, optionally one custom field |

Select Space ID before activating an event. The shared event model leaves the selector optional for SDK binding/manual mapping, but subscription creation validates that it is supplied. Custom field ID is optional and only applies to the custom-field event; it is ignored for the other two events. The app does not claim conditional field hiding.

Creation uses `POST /spaces/{spaceId}/webhooks`; filtered custom-field events use `parameterisedEvents` without duplicating the topic in `events`. Unsubscribe deletes only subscriptions with the exact Bird callback URL. Subscription validation checks existence and Active status using `GET /webhooks`. A temporarily unavailable API is surfaced as a service failure, not falsely reported as a deleted subscription.

Signed registration uses Wrike's `X-Hook-Secret` challenge and HMAC-SHA256 signature. The app verifies the body before answering the challenge, limits challenges to 1–100 alphanumeric characters, and compares signatures in constant time. Ordinary notifications are also signature-checked before a Flight is returned. This requires Blackbird to preserve the raw body string for verification; actual platform behavior remains to be tested. The SDK handshake interface acknowledges active subscriptions asynchronously with HTTP 202; an invalid payload then produces no Flight rather than changing an already-sent acknowledgment.

Object and array payloads are accepted. `[MultipleEvents]` returns one event per matching record, up to 100 matching records per delivery. Oversized deliveries fail explicitly rather than being truncated. A nonmatching topic/filter starts no Flight. This intentionally excludes Wrike's documented cross-tagging workaround where a TaskCreated subscription may receive a TaskParentsAdded payload: it is not represented as a genuinely new task.

Outputs: Task ID, Webhook ID, event type, author ID, change timestamp, current/previous status IDs where supplied, custom-field ID and current/previous value where supplied, and delivery ID from `Idempotency-Key`. Fields not sent by Wrike remain optional. Use Get task to retrieve the task's latest details.

Delivery is **at least once**, not exactly once. No in-memory cache is presented as durable duplicate suppression. For non-idempotent downstream steps, persist a deduplication key using the delivery ID plus the task/event identity; records within one delivered batch may share a delivery ID. No persistent event ledger is bundled. Wrike documents that mass actions/imports may not emit notifications. Webhooks can be suspended after failed deliveries. There is no historical replay, polling cursor or first-run baseline: polling validation requirements do not apply to this release.

## Input behavior and defaults

- Entity selectors show a readable label and the true underlying ID; custom-field option suggestions store the actual value. IDs remain ordinary scalar text or explicitly declared multi-value collections and accept manual/mapped values.
- Space → folder/task/form/custom field; space → workflow → custom status; task → attachment/comment/approval/timelog; custom field → its configured single-choice values.
- Missing required dropdown parents return empty choices. Folder and task suggestions are exceptions: omitted scoping IDs search the accessible account. All documented pages are searched before local filtering where the provider lacks search; task/folder/space/user handlers use documented server filters when available.
- Discovery is bounded at 10,000 fetched entities. If more data remains, the dropdown throws a narrowing message instead of claiming completeness. Most entity dropdowns show up to 100 matching suggestions; refine the search or map an ID. Accounts exceeding the catalogue bound may require mapping IDs directly.
- Task searches return up to 1,000 results by default; Maximum results accepts 1–10,000. Other guided searches return up to 10,000. Paging tokens are internal. Request forms use 100-item pages; groups use `pageToken`; task/timelog/folder pagination uses the provider's `nextPageToken` convention.
- No current user is silently selected as assignee or approver. Omitted create statuses use Wrike's own default. Omitted update fields are not sent. Status and Custom status ID are mutually exclusive.
- Updating task dates first reads the existing dates and preserves an omitted endpoint. A start date requires a due date; due-only creation is a milestone. Explicit date changes use second-level ISO date-time values. The app does not expose arbitrary scheduling-policy controls.
- Plain-text comments default to true. Timelog hours must be between 0 and 24. Approval behavior defaults to Wrike's server behavior; no approvers creates a draft, while supplying approvers can start an active approval. Users must choose this explicitly.
- No conditional input hiding, automatic dependent-field clearing, or custom Required/Advanced placement is claimed. After changing a parent selection, review child IDs; the provider ultimately validates access and compatibility.
- Nested payloads that vary by definition use documented advanced JSON inputs. The audit below explains these exceptions. They do not become automatically generated per-account form layouts.

## Files

Upload attachment accepts a Blackbird FileReference and sends raw file bytes to `POST /tasks/{taskId}/attachments` with the filename and binary-upload headers. FileManager's input stream is consumed once; there is no redundant MemoryStream copy. The current provider schema documents the endpoint but omits the binary header/body details; those follow Wrike support's published upload guidance and need live validation. The upload buffers one byte array; large files can encounter runtime memory limits before Wrike's published API attachment limit of 500 MB.

Download attachment retrieves native Wrike file metadata and bytes, then uploads the result to Blackbird's file service. External cloud attachments are rejected with a storage-provider fallback. Automatic HTTP redirects are disabled to avoid leaking bearer tokens; a deployment returning a redirect from the native download endpoint needs follow-up validation/handling. This is a known unverified behavior, not a claim that every attachment type works.

## Bulk, asynchronous work and retries

Update tasks in bulk accepts 1–100 task IDs and a shared custom status, assignee delta and/or custom-field payload. It uses the documented multi-task update, not a loop of individual changes. It does not expose unsupported bulk title/date changes.

Submit batch supports the documented `TaskCreate` and `FolderCreate` catalogue only, at most 100 operations. Each has `type`, `input`, and optional `clientId`. Example:

```json
[
  {
    "type": "TaskCreate",
    "clientId": "cms-item-42-fr",
    "input": {
      "folderId": "<mapped folder ID>",
      "title": "Localize CMS item 42 into French"
    }
  }
]
```

`clientId` is correlation, not an idempotency guarantee. Submit batch returns the job ID immediately so a caller can persist it. Wait for batch polls approximately every five seconds for up to roughly one minute, then returns the current state and same ID if still processing. Individual HTTP requests/retries can extend that elapsed time. Get batch result can resume later without resubmission. Status Failed and per-operation errors are returned for explicit branching; a Completed job can still contain operation failures. Result JSON retains the provider's complete dynamic per-operation result. Batches are not atomic and successful operations are not rolled back.

GET requests retry HTTP 429 up to three times, honoring Retry-After seconds/date values or using exponential backoff with jitter. A Retry-After longer than 60 seconds is surfaced rather than ignored. Writes, token exchanges, 5xx responses and ambiguous timeouts are not automatically retried. After an ambiguous write failure, inspect Wrike before rerunning the action. Read retries do not coordinate a shared tenant-wide rate limiter.

## Send request

Method uses a fixed GET/POST/PUT/DELETE catalogue. Path must be a relative v4 path, optionally starting with `/api/v4/`; external URLs, traversal, encoded path characters and inline query strings are rejected. Provide parameters as a JSON object; structured values are JSON encoded. `access_token` and method-override parameters are blocked. Optional Body JSON is for endpoints that document a JSON body and cannot be sent with GET. The response is raw response text in Response JSON; use the dedicated attachment actions for binary responses. Generic requests perform one call and do not automatically paginate arbitrary endpoints.

## Example Bird

1. CMS **On content created** emits the source content ID and URL.
2. Wrike **Create task**: choose the coordination space/folder, map the CMS title, and put source URL/content ID in Description or Metadata JSON.
3. An AI action localizes the source into explicitly selected target languages.
4. The CMS creates the new language variation.
5. Wrike **Update task custom field** writes the resulting CMS variation ID or URL into a selected field.
6. Wrike **Update task** sets the selected workflow's localized/completed custom status and updates metadata.
7. Optionally **Create comment** links to the translated variation or **Create approval** requests an explicit reviewer.

Persist the task ID from step 2 and reuse it. Neither target language nor publication status is inferred. If a later step fails, resume using the existing task ID rather than recreating the task.

## Repository layout and naming

Extract the source archive directly into the repository root. `Wrike.sln`, `Apps.Wrike/`, and `Tests.Wrike/` are at the archive root, without an enclosing directory. There are no `src/` or `tests/` directories. Solution references and project references use these root-level project folders.

The application display name and Product are `Wrike`. PackageId, AssemblyName and application RootNamespace are explicitly `Apps.Wrike`; the test RootNamespace is `Tests.Wrike`. No author-specific identifier is used. Version 1.0.4 preserves the existing app identity, action names, credential keys and saved input/output bindings.

## Build, installation and identity

```bash
dotnet restore Wrike.sln
dotnet build Wrike.sln --configuration Release
dotnet publish Apps.Wrike/Apps.Wrike.csproj --configuration Release --output publish
```

Zip the **contents** of publish with DLLs and the dependency manifest at the ZIP root. Do not use a runtime-specific publish flag. The delivered app ZIP uses this portable Release layout and is below Blackbird's documented size limit. The source ZIP is not the installable package.

For a new app: **Apps → My custom apps → Create app**, then upload `Wrike-1.0.4-app.zip`. The package's identifiers are Product `Wrike`, PackageId `Apps.Wrike` (explicitly set), AssemblyName `Apps.Wrike`; the application name is Wrike. The icon is a neutral W placeholder, not a verified official Wrike brand asset.

For updates: open the **existing app**, choose **New version → Update App**, and upload a package with a higher version. Keep Product, PackageId, AssemblyName, application/class/method names, credential keys and serialized property names stable. Do not create another app to update an installed one. Version 1.0.4 updates this app’s earlier releases; compatibility with independently developed Wrike apps is not established. If a system-wide identifier collision occurs on first install, select unique identifiers before initial deployment; do not change identifiers afterward to force an update.

## Validation and remaining gates

Completed locally:

- Portable .NET 8 Release build of app and test projects: zero warnings and zero errors.
- Metadata audit of 44 actions, 3 multiflight events and 15 dynamic handlers: required Display attributes present, data-source types implement the SDK interfaces, no duplicate static values or duplicate input names.
- 63 statically discoverable request-path references matched against the official OpenAPI specification or the official webhook guide, plus the three dynamically selected task-search routes. This checks path existence, not live semantics or complete parameter acceptance.
- Test discovery: 64 live-integration test cases, including one per action, dropdown checks, current-user connection validation and webhook lifecycle cases. Discovery is not test execution.
- Published portable package, ZIP layout and size checks.

Not verified:

- No live Wrike tests executed: no account credentials or disposable test resources were supplied. OAuth authorization/rotation, regional-host behavior, permissions, plan entitlements, API writes, pagination at scale and binary attachment transfer remain unverified live.
- No Blackbird installation/UI/runtime test executed: no target environment was supplied. Actual dropdown rendering, mapped inputs, parent injection, handshake routing, raw-body preservation, multiflight behavior and callback suspension/recovery remain unverified.
- Signature rejection, permissions, large catalogues, rate-limit headers and partial batch outcomes were reviewed in code; metadata inspection is not evidence that these behaviors have occurred against Wrike.

Live checks to run before production use:

1. Copy `Tests.Wrike/appsettings.json.example` to the ignored `appsettings.json`; populate only local credentials. Copy `sandbox.runsettings.example` to ignored `sandbox.runsettings`, selecting a disposable space/folder and, where needed, a disposable form, text custom field and already-active test user. Never use production routing forms.
2. Run `dotnet test Tests.Wrike/Tests.Wrike.csproj --settings Tests.Wrike/sandbox.runsettings`. Tests create temporary folders/tasks and clean up their resources; administration and request-form tests require explicit fixture flags. Review the existing async job after a timeout before rerunning. The FileManager is local test plumbing; Wrike API calls are real.
3. Webhook lifecycle tests require a dedicated public callback URL that implements this signed handshake. Do not point cleanup tests at a production Bird callback. Delivery-to-Flight tests must be run in Blackbird with disposable task changes.
4. Verify ordinary/admin profiles, wrong-region failure, token rotation, resource denial, pagination beyond the first page, search, missing parents, direct mapped IDs, raw-body signatures, duplicate deliveries, empty/nonmatching batches and each returned Flight.
5. Exercise a batch with one valid and one invalid operation and confirm partial outcomes are visible without replay. Validate timed-out writes by inspecting resources before retrying.

Tests intentionally require real credentials and fail setup if configuration is absent. They are not replaced by mocked provider responses. The SDK Common 2.16.0, Utils 1.1.0-rc.8709, FileManagement 1.1.0 and Polly 8.5.2 packages were resolved and compiled. The attached conventions take precedence over the older template layout; actual supported SDK categories and webhook interfaces were used rather than nonexistent sample enum values. Signed invalid requests are rejected even though a general webhook convention recommends HTTP 200 for normal preflight/no-match cases.

<!-- end docs -->

## Sources

Official API and SDK sources were consulted during implementation; the supplied `skills-main(3).zip` provided Blackbird implementation conventions.

- https://developers.wrike.com/openapi/wrike_api_v4_ver154.yaml
- https://developers.wrike.com/docs/oauth-20-authorization
- https://developers.wrike.com/docs/webhooks
- https://developers.wrike.com/reference/postbatchempty
- https://developers.wrike.com/docs/faq
- https://docs.blackbird.io/sdk/deploying/
- https://github.com/bb-io/TemplateApp
- https://github.com/bb-io/Asana
- https://help.wrike.com/hc/en-us/community/posts/360051318614-Add-attachments-to-tasks-with-API
