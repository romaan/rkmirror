import { Page, Locator } from '@playwright/test';

export class HomePage {
    readonly page: Page;
    readonly nameInput: Locator;
    readonly searchButton: Locator;
    readonly clearButton: Locator;
    readonly psychologistCards: Locator;
    readonly typeSelect: Locator;
    readonly paginationNext: Locator;

    constructor(page: Page) {
        this.page = page;
        this.nameInput = page.getByTestId('name-input');
        this.searchButton = page.getByTestId('search-button');
        this.clearButton = page.getByTestId('clear-button');
        this.psychologistCards = page.getByTestId('psychologist-card');
        this.typeSelect = page.getByTestId('type-select');
        this.paginationNext = page.getByTestId('pagination-next');
    }

    async goto() {
        await this.page.goto('/', { waitUntil: 'networkidle' });
    }

    async waitForResults() {
        const loading = this.page.getByText('Loading...', { exact: true });
        await loading.waitFor({ state: 'detached' }); // wait for loading text to disappear
    }


    async searchByName(name: string | undefined) {
        if (typeof name !== 'string') {
            throw new Error('searchByName: name must be a string');
        }
        await this.nameInput.fill(name);
        await this.searchButton.click();
    }


    async filterByType(type: string) {
        await this.typeSelect.selectOption(type);
        await this.searchButton.click();
    }

    async clearFilters() {
        await this.clearButton.click();
    }

    async goToNextPage() {
        await this.paginationNext.click();
    }

    async getCardTexts() {
        return this.psychologistCards.allTextContents();
    }
}
