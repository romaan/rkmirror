import { faker } from '@faker-js/faker';
import sql from 'mssql';
import dotenv from 'dotenv';
import { v4 as uuidv4 } from 'uuid';
import { Psychologist } from '@types/psychologist';
import {PsychologistTypeEnum} from '@types/psychologist-type';

dotenv.config();

let pool: sql.ConnectionPool | null = null;

const dbConfig: sql.config = {
    user: process.env.MSSQL_USER,
    password: process.env.MSSQL_PASSWORD,
    server: process.env.MSSQL_SERVER || 'localhost',
    port: Number(process.env.MSSQL_PORT || 1433),
    database: process.env.MSSQL_DATABASE,
    options: {
        encrypt: false,
        trustServerCertificate: true,
    },
};

/**
 * Connect to the MSSQL database and reuse the connection.
 *
 */
export async function connectDb(): Promise<sql.ConnectionPool> {
    if (pool && pool.connected) {
        return pool;
    }

    if (pool && !pool.connected) {
        try {
            await pool.connect();
            return pool;
        } catch (err) {
            console.warn('Reconnection failed, creating new pool.');
        }
    }

    pool = await new sql.ConnectionPool({
        user: process.env.MSSQL_USER,
        password: process.env.MSSQL_PASSWORD,
        server: process.env.MSSQL_SERVER || 'localhost',
        port: parseInt(process.env.MSSQL_PORT || '1433', 10),
        database: process.env.MSSQL_DATABASE,
        options: {
            trustServerCertificate: true // for local dev with self-signed certs
        }
    }).connect();

    return pool;
}

/**
 * Reset the database between tests.
 */
export async function resetDatabase() {
    const pool = await connectDb();
    try {
        await pool.request().query(`
          DELETE FROM dbo.AvailableDates;
          DELETE FROM dbo.Psychologists;
        `);
    } finally {
        await pool.close();
    }
}


/**
 * Seed N generic psychologists with random names and types.
 */
export async function seedPsychologists(count = 25) {
    const pool = await connectDb();
    const typeLabels = Object.keys(PsychologistTypeEnum); // e.g., ['Clinical', 'General', 'Anxiety', 'Depression']

    for (let i = 0; i < count; i++) {
        const transaction = pool.transaction();
        await transaction.begin();

        try {
            const id = uuidv4();
            const firstName = faker.person.firstName();
            const lastName = faker.person.lastName();
            const typeLabel = typeLabels[i % typeLabels.length];
            const typeValue = PsychologistTypeEnum[typeLabel];
            const desc = faker.lorem.sentence({ min: 8, max: 12 });

            // One request for psychologist insert
            const request = transaction.request();

            await request
                .input('Id', sql.UniqueIdentifier, id)
                .input('FirstName', sql.NVarChar(100), firstName)
                .input('LastName', sql.NVarChar(100), lastName)
                .input('PsychologistType', sql.Int, typeValue)
                .input('ShortDescription', sql.NVarChar(500), desc)
                .query(`
                    INSERT INTO dbo.Psychologists (Id, FirstName, LastName, PsychologistType, ShortDescription)
                    VALUES (@Id, @FirstName, @LastName, @PsychologistType, @ShortDescription)
                `);

            const availableDays = faker.number.int({ min: 2, max: 5 });

            for (let j = 0; j < availableDays; j++) {
                const date = faker.date.soon({ days: 14 });

                // Create a NEW request for each insert into AvailableDates
                const dateRequest = transaction.request();
                await dateRequest
                    .input('PsychologistId', sql.UniqueIdentifier, id)
                    .input('Date', sql.DateTime2, date)
                    .query(`
                        INSERT INTO dbo.AvailableDates (PsychologistId, Date)
                        VALUES (@PsychologistId, @Date)
                    `);
            }

            await transaction.commit();
            console.log(`Seeded psychologist ${firstName} ${lastName}`);
        } catch (error) {
            await transaction.rollback();
            console.error(`Transaction failed for psychologist ${i + 1}:`, error.message);
        }
    }
}


export async function createPsychologist({
                                             firstName,
                                             lastName,
                                             type,
                                             description,
                                             dates
                                         }: {
    firstName: string;
    lastName: string;
    type: number;
    description: string;
    dates: string[]; // ISO strings
}): Promise<Psychologist> {
    const pool = await connectDb();
    const transaction = new sql.Transaction(pool);
    const id = uuidv4();

    if (typeof type !== 'number' || isNaN(type)) {
        throw new Error('Invalid psychologist type');
    }

    try {
        await transaction.begin();

        const request = new sql.Request(transaction);
        await request
            .input('Id', sql.UniqueIdentifier, id)
            .input('FirstName', sql.NVarChar(100), firstName)
            .input('LastName', sql.NVarChar(100), lastName)
            .input('PsychologistType', sql.Int, type)
            .input('ShortDescription', sql.NVarChar(500), description)
            .query(`
        INSERT INTO dbo.Psychologists (Id, FirstName, LastName, PsychologistType, ShortDescription)
        VALUES (@Id, @FirstName, @LastName, @PsychologistType, @ShortDescription)
      `);

        for (const date of dates) {
            const dateRequest = new sql.Request(transaction);
            await dateRequest
                .input('PsychologistId', sql.UniqueIdentifier, id)
                .input('Date', sql.DateTime2, new Date(date))
                .query(`
          INSERT INTO dbo.AvailableDates (PsychologistId, Date)
          VALUES (@PsychologistId, @Date)
        `);
        }

        await transaction.commit();

        return {
            Id: id,
            FirstName: firstName,
            LastName: lastName,
            PsychologistType: type,
            ShortDescription: description
        };
    } catch (error) {
        await transaction.rollback();
        console.error('Transaction rolled back due to error:', error);
        throw error;
    } finally {
        await pool.close();
    }
}

/**
 * Optional cleanup
 */
export async function closeDb() {
    if (pool) {
        await pool.close();
    }
}