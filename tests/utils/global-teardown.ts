// tests/global-teardown.ts
import { resetDatabase } from '@utils/db';
import { connectDb } from '@utils/db';

export default async () => {
    const db = await connectDb();
    await resetDatabase(db);
    await db.close();
};
