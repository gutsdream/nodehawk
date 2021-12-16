export interface CreateNodeRequest {
    title: string;
    externalId: string | undefined;
    host: string;
    username: string;
    key: string;
}