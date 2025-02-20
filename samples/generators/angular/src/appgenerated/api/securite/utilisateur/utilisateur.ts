////
//// ATTENTION CE FICHIER EST GENERE AUTOMATIQUEMENT !
////


import {HttpClient, HttpParams} from "@angular/common/http";
import {Injectable} from "@angular/core";
import {Observable} from "rxjs";
import {TypeUtilisateurCode} from "../../../model/securite/utilisateur/references";
import {UtilisateurItem} from "../../../model/securite/utilisateur/utilisateur-item";
import {UtilisateurRead} from "../../../model/securite/utilisateur/utilisateur-read";
import {UtilisateurWrite} from "../../../model/securite/utilisateur/utilisateur-write";
@Injectable({
    providedIn: 'root'
})
export class UtilisateurService {

    constructor(private http: HttpClient) {}

    /**
     * Ajoute un utilisateur
     * @param utilisateur Utilisateur à sauvegarder
     * @param options Options pour 'fetch'.
     * @returns Utilisateur sauvegardé
     */
    addUtilisateur(utilisateur: UtilisateurWrite): Observable<UtilisateurRead> {
        return this.http.post<UtilisateurRead>(`/api/utilisateurs`, utilisateur);
    }

    /**
     * Supprime un utilisateur
     * @param utiId Id de l'utilisateur
     * @param options Options pour 'fetch'.
     */
    deleteUtilisateur(utiId: number): Observable<void> {
        return this.http.delete<void>(`/api/utilisateurs/${utiId}`);
    }

    /**
     * Charge le détail d'un utilisateur
     * @param utiId Id de l'utilisateur
     * @param options Options pour 'fetch'.
     * @returns Le détail de l'utilisateur
     */
    getUtilisateur(utiId: number): Observable<UtilisateurRead> {
        return this.http.get<UtilisateurRead>(`/api/utilisateurs/${utiId}`);
    }

    /**
     * Recherche des utilisateurs
     * @param nom Nom de l'utilisateur
     * @param prenom Nom de l'utilisateur
     * @param email Email de l'utilisateur
     * @param dateNaissance Age de l'utilisateur
     * @param actif Si l'utilisateur est actif
     * @param profilId Profil de l'utilisateur
     * @param typeUtilisateurCode Type d'utilisateur
     * @param options Options pour 'fetch'.
     * @returns Utilisateurs matchant les critères
     */
    searchUtilisateur(nom?: string, prenom?: string, email?: string, dateNaissance?: string, actif?: boolean, profilId?: number, typeUtilisateurCode?: TypeUtilisateurCode, queryParams: any = {}): Observable<UtilisateurItem[]> {
        if(nom) {
            queryParams['nom'] = nom
        }

        if(prenom) {
            queryParams['prenom'] = prenom
        }

        if(email) {
            queryParams['email'] = email
        }

        if(dateNaissance) {
            queryParams['dateNaissance'] = dateNaissance
        }

        if(actif) {
            queryParams['actif'] = actif
        }

        if(profilId) {
            queryParams['profilId'] = profilId
        }

        if(typeUtilisateurCode) {
            queryParams['typeUtilisateurCode'] = typeUtilisateurCode
        }

        const httpParams = new HttpParams({fromObject : queryParams});
        const httpOptions = { params: httpParams }

        return this.http.get<UtilisateurItem[]>(`/api/utilisateurs`, httpOptions);
    }

    /**
     * Sauvegarde un utilisateur
     * @param utiId Id de l'utilisateur
     * @param utilisateur Utilisateur à sauvegarder
     * @param options Options pour 'fetch'.
     * @returns Utilisateur sauvegardé
     */
    updateUtilisateur(utiId: number, utilisateur: UtilisateurWrite): Observable<UtilisateurRead> {
        return this.http.put<UtilisateurRead>(`/api/utilisateurs/${utiId}`, utilisateur);
    }
}
