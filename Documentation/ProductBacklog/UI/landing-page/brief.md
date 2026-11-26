# Landing page implementation brief

## Goal

Replace the current `/` welcome screen with the public landing page shown in the prototype. The page should introduce Recipe Manager and let visitors explore its main features or continue to sign in or register.

## Reference

- [Domain landing page prototype](<User stories/domain-landing.html>)
- [Approved product decisions](decisions.md)

Use the prototype as a visual starting point only. The approved decisions in this context and the login-registration context take precedence over prototype details, including its public-header authentication links and route-based account actions.

## Current experience

`RecipeManager.UI/Features/Landing/LandingPage.razor` currently shows a welcome heading, a short description, and links to account registration and sign-in.

## Intended page

Update the `/` page using the approved decisions:

- Keep the Recipe Manager brand and Features navigation; omit About us and public-header authentication links.
- Keep the hero message and account actions.
- Place labeled Sign in and Register actions with the hero; both open the shared authentication dialog in the corresponding form mode.
- Retain the prototype's section labels for Dishes, Recipes, Cookbooks, Meal Plan, and shared units and categories. Use placeholders for section content; detailed component shapes are not defined yet.

## Scope boundary

This work updates the landing-page experience only. It does not add or change recipe-management, account, or meal-planning capabilities.

## Backlog status

The approved PRD and draft User Story are in this context. The prototype is beside the Story under `User stories`.
