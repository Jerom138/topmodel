////
//// ATTENTION CE FICHIER EST GENERE AUTOMATIQUEMENT !
////

package topmodel.exemple.name.dao.dtos.securite.utilisateur.interfaces;

import javax.annotation.Generated;
import javax.validation.constraints.Email;

import topmodel.exemple.name.dao.dtos.securite.utilisateur.UtilisateurDto;
import topmodel.exemple.name.dao.entities.securite.utilisateur.TypeUtilisateur;

@Generated("TopModel : https://github.com/klee-contrib/topmodel")
public interface IUtilisateurDto {

	/**
	 * Getter for id.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.securite.utilisateur.UtilisateurDto#id id}.
	 */
	long getId();

	/**
	 * Getter for email.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.securite.utilisateur.UtilisateurDto#email email}.
	 */
	String getEmail();

	/**
	 * Getter for typeUtilisateurCode.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.securite.utilisateur.UtilisateurDto#typeUtilisateurCode typeUtilisateurCode}.
	 */
	TypeUtilisateur.Values getTypeUtilisateurCode();

	/**
	 * Getter for utilisateurParent.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.securite.utilisateur.UtilisateurDto#utilisateurParent utilisateurParent}.
	 */
	UtilisateurDto getUtilisateurParent();
}
