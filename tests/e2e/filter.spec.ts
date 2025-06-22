import { expect } from '@playwright/test';
import { test } from '@fixtures/db-fixture';
import { HomePage } from '@pages/HomePage';
import { createPsychologist } from '@utils/db';
import { Psychologist } from '@types/psychologist';
import { PsychologistTypeEnum } from '../types/psychologist-type';

test.describe('Filtering', () => {
    let specialPsychologist: Psychologist;

    test('filters by name', async ({ page }) => {
        specialPsychologist = await createPsychologist({
            firstName: 'Romaan',
            lastName: 'Wonderland',
            type: PsychologistTypeEnum.Clinical,
            description: 'Special test psychologist',
            dates: [new Date().toISOString(), new Date(Date.now() + 86400000).toISOString()]
        });

        const home = new HomePage(page);
        await home.goto();

        await home.searchByName(specialPsychologist.FirstName);
        await home.waitForResults();

        const cardTexts = await home.getCardTexts();
        expect(cardTexts[0]).toContain(specialPsychologist.FirstName);
    });

    test('filters by type', async ({ page }) => {
        const home = new HomePage(page);
        await home.goto();

        await home.filterByType('Clinical');
        await home.waitForResults();

        const cardTexts = await home.getCardTexts();
        expect(cardTexts.length).toBeGreaterThan(0);
        expect(cardTexts.every(text => text.includes('Clinical'))).toBeTruthy();
    });

    test('clears filters', async ({ page }) => {
        const home = new HomePage(page);
        await home.goto();

        await home.searchByName('Bob');
        await home.clearFilters();
        await home.waitForResults();

        await expect(home.nameInput).toHaveValue('');
    });
});
