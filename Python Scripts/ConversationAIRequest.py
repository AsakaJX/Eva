from playwright.async_api import async_playwright
import time
import sys

from playwright.sync_api import Playwright, sync_playwright, expect


def run(playwright: Playwright) -> None:
    print(f'### Requested phrase ###\n{sys.argv[1]}\n')
    browser = playwright.chromium.launch(headless=True)
    context = browser.new_context()
    page = context.new_page()
    page.goto("https://f5a62bada5ac4069f9.gradio.live/")

    page.get_by_role("button", name="Clear history").click()
    page.get_by_role("button", name="Confirm").click()

    page.locator("#component-272").click()
    page.locator("#component-276 button").first.click()

    page.get_by_label("Input", exact=True).click()
    page.get_by_label("Input", exact=True).fill(sys.argv[1])
    page.get_by_label("Input", exact=True).press("Enter")

    chat_response_locator = "#chat div"
    page.wait_for_selector(chat_response_locator)

    # input()

    downBad = [
        "Is typing...",
        "You join Eva's stream She acknowledges you and becomes incredibly excited and happy while she's waiting for your message Hii! I'm very happy to see you today on my stream!"
    ]

    counter = 0
    repeat_counter = 0
    is_typing_flag = False
    prev_msg = "NOTHING"
    while True:
        time.sleep(1)
        
        counter += 1
        continueFlag = False

        msg = page.locator(chat_response_locator).all_inner_texts()[0].strip().replace("\n","").removeprefix("EVA")
        if msg == "Is typing...":
            is_typing_flag = True
        
        if continueFlag or msg in downBad:
            continue

        if is_typing_flag and counter % 2 == 0 or counter == 1:
            prev_msg = msg

        if is_typing_flag and prev_msg == msg:
            repeat_counter += 1
        else:
            repeat_counter = 0

        if is_typing_flag and repeat_counter == 3:
            print(f'\n\n\nEVA_FINAL: \n{msg}')
            break

        for i in range(0, len(msg)):
            if i + 1 != len(msg) and msg[i] == ' ' and msg[i + 1] == ' ':
                continueFlag = True
                break

        print(f'EVA_RESPONSE: {msg}')
        
    # ---------------------
    # input()
    context.close()
    browser.close()

with sync_playwright() as playwright:
    run(playwright)