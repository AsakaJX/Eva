from playwright.sync_api import Playwright, sync_playwright, expect
import time
import sys

def run(playwright: Playwright) -> None:
    browser = playwright.chromium.launch(headless=True)
    context = browser.new_context()
    page = context.new_page()
    page.goto(f'https://www.deepl.com/translator#auto/{sys.argv[1]}/{sys.argv[2]}')

    while True:
        translated = page.get_by_test_id("translator-target-input").get_by_role("paragraph").all_text_contents()[0]
        if len(translated) > 0:
            print(translated)
            break

    context.close()
    browser.close()

with sync_playwright() as playwright:
    run(playwright)
