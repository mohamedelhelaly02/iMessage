import { HttpClient } from "@angular/common/http";
import { inject, Service } from "@angular/core";
import { environment } from "../../../../environments/environment";
import { Observable } from "rxjs";
import { IConversation } from "../conversation.models";

@Service()
export class ConversationApi {
    private readonly BASE_URL = `${environment.apiUrl}/conversations`;
    private readonly httpClient = inject(HttpClient);

    getConversation(): Observable<IConversation[]> {
        return this.httpClient.get<IConversation[]>(this.BASE_URL);
    }
}