// tests/global-setup.ts
import { seedPsychologists } from '@utils/db';
import { connectDb } from '@utils/db';

export default async () => {
    const db = await connectDb();
    await seedPsychologists(25, db);
    await db.close();
};
