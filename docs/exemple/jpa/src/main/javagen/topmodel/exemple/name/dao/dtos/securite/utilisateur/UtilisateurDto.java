////
//// ATTENTION CE FICHIER EST GENERE AUTOMATIQUEMENT !
////

package topmodel.exemple.name.dao.dtos.securite.utilisateur;

import java.io.Serializable;

import javax.annotation.Generated;
import javax.validation.constraints.Email;
import javax.validation.constraints.NotNull;

import topmodel.exemple.name.dao.dtos.securite.utilisateur.interfaces.IUtilisateurDto;
import topmodel.exemple.name.dao.entities.securite.utilisateur.TypeUtilisateur;
import topmodel.exemple.name.dao.entities.securite.utilisateur.Utilisateur;

/**
 * Objet non persisté de communication avec le serveur.
 */
@Generated("TopModel : https://github.com/klee-contrib/topmodel")
public class UtilisateurDto implements Serializable, IUtilisateurDto {
	/** Serial ID */
	private static final long serialVersionUID = 1L;

	/**
	 * Id technique.
	 * Alias of {@link topmodel.exemple.name.dao.entities.securite.utilisateur.Utilisateur#getId() Utilisateur#getId()} 
	 */
	private long id;

	/**
	 * Email de l'utilisateur.
	 * Alias of {@link topmodel.exemple.name.dao.entities.securite.utilisateur.Utilisateur#getEmail() Utilisateur#getEmail()} 
	 */
	@NotNull
	@Email
	private String email;

	/**
	 * Type d'utilisateur en Many to one.
	 * Alias of {@link topmodel.exemple.name.dao.entities.securite.utilisateur.Utilisateur#getTypeUtilisateurCode() Utilisateur#getTypeUtilisateurCode()} 
	 */
	@NotNull
	private TypeUtilisateur.Values typeUtilisateurCode;

	/**
	 * UtilisateurParent.
	 */
	private UtilisateurDto utilisateurParent;

	/**
	 * No arg constructor.
	 */
	public UtilisateurDto() {
	}

	/**
	 * Copy constructor.
	 * @param utilisateurDto to copy
	 */
	public UtilisateurDto(UtilisateurDto utilisateurDto) {
		if(utilisateurDto == null) {
			return;
		}

		this.id = utilisateurDto.getId();
		this.email = utilisateurDto.getEmail();
		this.typeUtilisateurCode = utilisateurDto.getTypeUtilisateurCode();
		this.utilisateurParent = utilisateurDto.getUtilisateurParent();
	}

	/**
	 * All arg constructor.
	 * @param id Id technique
	 * @param email Email de l'utilisateur
	 * @param typeUtilisateurCode Type d'utilisateur en Many to one
	 * @param utilisateurParent UtilisateurParent
	 */
	public UtilisateurDto(long id, String email, TypeUtilisateur.Values typeUtilisateurCode, UtilisateurDto utilisateurParent) {
		this.id = id;
		this.email = email;
		this.typeUtilisateurCode = typeUtilisateurCode;
		this.utilisateurParent = utilisateurParent;
	}

	/**
	 * Crée une nouvelle instance de 'UtilisateurDto'.
	 * @param utilisateur Instance de 'Utilisateur'.
	 *
	 * @return Une nouvelle instance de 'UtilisateurDto'.
	 */
	public UtilisateurDto(Utilisateur utilisateur) {
		this.from(utilisateur);
	}

	/**
	 * Map les champs des classes passées en paramètre dans l'instance courante.
	 * @param utilisateur Instance de 'Utilisateur'.
	 */
	protected void from(Utilisateur utilisateur) {
		if(utilisateur != null) {
			this.id = utilisateur.getId();
			this.email = utilisateur.getEmail();

			if(utilisateur.getTypeUtilisateur() != null) {
				this.typeUtilisateurCode = utilisateur.getTypeUtilisateur().getCode();
			}

		}

	}

	/**
	 * Getter for id.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.securite.utilisateur.UtilisateurDto#id id}.
	 */
	@Override
	public long getId() {
		return this.id;
	}

	/**
	 * Getter for email.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.securite.utilisateur.UtilisateurDto#email email}.
	 */
	@Override
	public String getEmail() {
		return this.email;
	}

	/**
	 * Getter for typeUtilisateurCode.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.securite.utilisateur.UtilisateurDto#typeUtilisateurCode typeUtilisateurCode}.
	 */
	@Override
	public TypeUtilisateur.Values getTypeUtilisateurCode() {
		return this.typeUtilisateurCode;
	}

	/**
	 * Getter for utilisateurParent.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.securite.utilisateur.UtilisateurDto#utilisateurParent utilisateurParent}.
	 */
	@Override
	public UtilisateurDto getUtilisateurParent() {
		return this.utilisateurParent;
	}

	/**
	 * Set the value of {@link topmodel.exemple.name.dao.dtos.securite.utilisateur.UtilisateurDto#id id}.
	 * @param id value to set
	 */
	public void setId(long id) {
		this.id = id;
	}

	/**
	 * Set the value of {@link topmodel.exemple.name.dao.dtos.securite.utilisateur.UtilisateurDto#email email}.
	 * @param email value to set
	 */
	public void setEmail(String email) {
		this.email = email;
	}

	/**
	 * Set the value of {@link topmodel.exemple.name.dao.dtos.securite.utilisateur.UtilisateurDto#typeUtilisateurCode typeUtilisateurCode}.
	 * @param typeUtilisateurCode value to set
	 */
	public void setTypeUtilisateurCode(TypeUtilisateur.Values typeUtilisateurCode) {
		this.typeUtilisateurCode = typeUtilisateurCode;
	}

	/**
	 * Set the value of {@link topmodel.exemple.name.dao.dtos.securite.utilisateur.UtilisateurDto#utilisateurParent utilisateurParent}.
	 * @param utilisateurParent value to set
	 */
	public void setUtilisateurParent(UtilisateurDto utilisateurParent) {
		this.utilisateurParent = utilisateurParent;
	}

	/**
	 * Mappe 'UtilisateurDto' vers 'Utilisateur'.
	 * @param source Instance de 'UtilisateurDto'.
	 * @param dest Instance pré-existante de 'Utilisateur'. Une nouvelle instance sera créée si non spécifié.
	 *
	 * @return Une instance de 'Utilisateur'.
	 */
	public Utilisateur toUtilisateur(Utilisateur dest) {
		dest = dest == null ? new Utilisateur() : dest;

		dest.setId(this.getId());
		dest.setEmail(this.getEmail());

		return dest;
	}
}
