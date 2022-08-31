////
//// ATTENTION CE FICHIER EST GENERE AUTOMATIQUEMENT !
////

import {DO_CODE, DO_EMAIL, DO_ID} from "@domains";

import {TypeUtilisateurCode} from "./references";

export interface UtilisateurDto {
    id?: number,
    email?: string,
    typeUtilisateurCode?: TypeUtilisateurCode,
    utilisateurParent?: UtilisateurDto
}

export const UtilisateurDtoEntity = {
    id: {
        type: "field",
        name: "id",
        domain: DO_ID,
        isRequired: false,
        label: "securite.utilisateur.utilisateur.id"
    },
    email: {
        type: "field",
        name: "email",
        domain: DO_EMAIL,
        isRequired: true,
        label: "securite.utilisateur.utilisateur.email"
    },
    typeUtilisateurCode: {
        type: "field",
        name: "typeUtilisateurCode",
        domain: DO_CODE,
        isRequired: true,
        label: "securite.utilisateur.utilisateur.typeUtilisateurCode"
    },
    utilisateurParent: {
        type: "object",
    }
} as const
