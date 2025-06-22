import { test } from '@fixtures/db-fixture';
import { expect } from '@playwright/test';
import { HomePage } from '@pages/HomePage';


test.describe('Homepage', () => {
    test('displays psychologist cards and supports pagination', async ({ page }) => {
        const home = new HomePage(page);
        await home.goto();

        const count = await home.psychologistCards.count();
        expect(count).toBeGreaterThan(0);

        if (await home.paginationNext.isVisible()) {
            await home.goToNextPage();
            await home.waitForResults();

            // Assert that new cards are loaded (you can improve this by comparing before/after)
            const newCardCount = await home.getCardTexts();
            expect(newCardCount.length).toBeGreaterThan(0);
        }
    });
});
