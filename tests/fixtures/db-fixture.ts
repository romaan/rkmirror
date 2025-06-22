// tests/fixtures/db-fixture.ts
import { test as base } from '@playwright/test';
import { connectDb } from '@utils/db';
import type { ConnectionPool } from 'mssql';

type TestFixtures = {
    db: ConnectionPool;
};

export const test = base.extend<TestFixtures>({
    db: async ({}, use) => {
        const pool = await connectDb();   // setup
        await use(pool);                  // make available as `db` in tests
        await pool.close();              // teardown
    },
});
