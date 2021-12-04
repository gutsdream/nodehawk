// Node is taken :(
export interface OtNode {
    id: string;
    title: string;
    externalId: string;
    spaceUsedPercentage: number;
    containerRunning: boolean;
    lastBackupDateUtc: Date;
    lastCleanedDateUtc: Date;
    lastSnapshotDateUtc: Date;
    createdDateUtc: Date;
    minutesSinceLastSnapshot: number;
}