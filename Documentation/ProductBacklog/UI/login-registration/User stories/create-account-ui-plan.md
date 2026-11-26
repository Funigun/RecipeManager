---
status: draft
story: Documentation/ProductBacklog/UI/login-registration/User stories/create-account.md
---
# UI plan: Create an account

## Sources
| Item | Origin | Why needed now |
|---|---|---|
| Shared dialog registration mode and mode switching | `Documentation/ProductBacklog/UI/login-registration/User stories/choose-authentication-form.md`, AC-01 and AC-03: landing Register opens registration mode and the visitor can switch forms in-dialog. | Registration is hosted by the shared dialog and its success transition must remain within that dialog. |
| Registration input rules | `Documentation/ProductBacklog/UI/login-registration/prd.md`, R-02: username (maximum 15 characters), valid email, password (minimum 10 characters with lowercase, uppercase, digit and special character); R-03: confirmation must match and is not in the request. | Supplies the exact field and validation requirements referenced by this Story's AC-05 and AC-06. |
| Registration success outcome | `Documentation/ProductBacklog/UI/login-registration/prd.md`, R-04: show a success message, switch to Sign in, and do not sign the visitor in. | Defines the outcome after the API reports success. |

## Consumed API
| Endpoint (method + route) | Origin (contract / OpenAPI / code path) | DTOs | Used by |
|---|---|---|---|
| `POST /api/account/register` | `RecipeManager.Identity.API/Features/Account/AccountGroup.cs` (`RegisterRoute`) and `Features/Account/UserRegistration.cs` endpoint/handler. | Request: `UserName`, `Password`, `Email`; endpoint handler returns success without a response body. Confirmation password is not part of the request. | Registration workflow in the shared authentication dialog. |

## Backend gaps
- No endpoint or request-field gaps. Reusable client-side validation is a cross-project contract issue recorded under Risks and open questions.

## Scope
Registration mode and its workflow in the shared account dialog. Existing `/register` page is a separate routable component and is not the target experience. Use a registration-specific UI form model including confirmation, map to the API request at the workflow boundary, and keep HTTP/error interpretation out of presentation-only form components. Current UI has no identity API client or account-form workflow. Use the approved prototype only for form-layout reference; its link to a separate page is not route behavior.

## File structure
```text
RecipeManager.UI/
├── Features/Account/Register/RegisterView.razor [create] — registration workflow and API result handling
├── Features/Account/Register/Components/RegistrationForm.razor [create] — fields and callbacks, without API orchestration
├── Features/Account/Register/Models/RegistrationFormModel.cs [create] — registration input including UI-only confirmation
├── Features/Account/Register/RegistrationApiClient.cs [create] — focused client for POST /api/account/register
├── Features/Account/Shared/AuthenticationDialog.razor [modify] — render registration workflow in registration mode
└── Program.cs [modify] — register the registration API client
```

## Tasks
| ID | Description | Files/area | Depends on | Covers (AC-/R-) |
|---|---|---|---|---|
| U-1 | Add the registration form model and form component with username, email, password and confirmation fields. Keep confirmation out of the API request; implement confirmation-match validation. R-02 client validation is blocked by the shared-validator gap below. | `RecipeManager.UI/Features/Account/Register/Models/RegistrationFormModel.cs`, `RecipeManager.UI/Features/Account/Register/Components/RegistrationForm.razor` | Shared dialog from `choose-authentication-form` | AC-05, AC-06 |
| U-2 | Add the focused registration client and map only `UserName`, `Email` and `Password` to the existing endpoint. Preserve form state on failure and reset submitting state on all outcomes. | `RecipeManager.UI/Features/Account/Register/RegistrationApiClient.cs`, `RecipeManager.UI/Features/Account/Register/RegisterView.razor`, `RecipeManager.UI/Program.cs` | U-1; resolve Identity API base address | AC-05 |
| U-3 | Render the registration workflow in the shared dialog; on endpoint success show confirmation and switch the same dialog to Sign in without establishing a session. | `RecipeManager.UI/Features/Account/Register/RegisterView.razor`, `RecipeManager.UI/Features/Account/Shared/AuthenticationDialog.razor` | U-2 | AC-07 |
| U-4 | Provide a responsive registration form and an error-display slot without choosing the unresolved error presentation. | `RecipeManager.UI/Features/Account/Register/Components/RegistrationForm.razor` | U-1–U-3; error presentation decision | AC-05–AC-07 |

## Test plan
| Level | What is tested | Covers | Task |
|---|---|---|---|
| Manual browser/UI | Username, email, password and confirmation fields render; test R-02 client enforcement only after a shared-validator decision. Confirm that confirmation is absent from the outbound request. | AC-05 | U-1, U-2 |
| Manual browser/UI | Mismatched confirmation prevents request submission. | AC-06 | U-1 |
| Manual network/contract | Registration sends only `UserName`, `Email` and `Password`; success is interpreted without assuming a response body. | AC-05 | U-2 |
| Manual browser/UI | Successful registration shows confirmation and switches to Sign in in the same dialog; no sign-in request/session is started. | AC-07 | U-3 |
| Manual browser/network | Submitting state resets on both success and failure; request failure does not erase the user's form values. | AC-05–AC-07 | U-2, U-4 |
| Manual responsive | Registration fields and primary action remain usable at phone and desktop widths. | AC-05–AC-07 | U-4 |

## Risks and open questions
- **Unresolved product question:** the user-visible presentation of registration validation failures and unexpected request errors is not established by the PRD or target Story. Keep this decision open; do not choose inline, summary, toast, or other presentation silently.
- The Identity API validates username, email and password composition, but those validators are server-only. `RecipeManager.Shared.Contracts` has no source project or shared validators, so client enforcement of R-02 needs a shared-contract decision; do not duplicate the server rules.
- The UI currently has no localization resources or localization registration, while the Blazor instructions require resource-backed application text. Decide the resource setup before adding localized field, validation and status messages.
- `Program.cs` currently gives `HttpClient` the UI's base address. Confirm how the Aspire gateway exposes the Identity API to the browser before setting the registration client's base address.
