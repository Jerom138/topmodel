////
//// ATTENTION CE FICHIER EST GENERE AUTOMATIQUEMENT !
////

package topmodel.exemple.name.dao.dtos.utilisateur;

import java.io.Serializable;

import javax.annotation.Generated;
import javax.validation.constraints.Email;
import javax.validation.constraints.NotNull;

import topmodel.exemple.name.dao.dtos.utilisateur.interfaces.IUtilisateurDto;
import topmodel.exemple.name.dao.entities.utilisateur.TypeUtilisateur;
import topmodel.exemple.name.dao.entities.utilisateur.Utilisateur;

/**
 * Objet non persisté de communication avec le serveur.
 */
@Generated("TopModel : https://github.com/klee-contrib/topmodel")
public class UtilisateurDto implements Serializable, IUtilisateurDto {
	/** Serial ID */
	private static final long serialVersionUID = 1L;

	/**
	 * Id technique.
	 * Alias of {@link topmodel.exemple.name.dao.entities.utilisateur.Utilisateur#getId() Utilisateur#getId()} 
	 */
	@NotNull
	private long utilisateurId;

	/**
	 * Email de l'utilisateur.
	 * Alias of {@link topmodel.exemple.name.dao.entities.utilisateur.Utilisateur#getEmail() Utilisateur#getEmail()} 
	 */
	@NotNull
	@Email
	private String utilisateurEmail;

	/**
	 * Type d'utilisateur en Many to one.
	 * Alias of {@link topmodel.exemple.name.dao.entities.utilisateur.Utilisateur#getTypeUtilisateurCode() Utilisateur#getTypeUtilisateurCode()} 
	 */
	@NotNull
	private TypeUtilisateur.Values utilisateurTypeUtilisateurCode;

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

		this.utilisateurId = utilisateurDto.getUtilisateurId();
		this.utilisateurEmail = utilisateurDto.getUtilisateurEmail();
		this.utilisateurTypeUtilisateurCode = utilisateurDto.getUtilisateurTypeUtilisateurCode();
		this.utilisateurParent = utilisateurDto.getUtilisateurParent();
	}

	/**
	 * All arg constructor.
	 * @param utilisateurId Id technique
	 * @param utilisateurEmail Email de l'utilisateur
	 * @param utilisateurTypeUtilisateurCode Type d'utilisateur en Many to one
	 * @param utilisateurParent UtilisateurParent
	 */
	public UtilisateurDto(long utilisateurId, String utilisateurEmail, TypeUtilisateur.Values utilisateurTypeUtilisateurCode, UtilisateurDto utilisateurParent) {
		this.utilisateurId = utilisateurId;
		this.utilisateurEmail = utilisateurEmail;
		this.utilisateurTypeUtilisateurCode = utilisateurTypeUtilisateurCode;
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
			this.utilisateurId = utilisateur.getId();
			this.utilisateurEmail = utilisateur.getEmail();

			if(utilisateur.getTypeUtilisateur() != null) {
				this.utilisateurTypeUtilisateurCode = utilisateur.getTypeUtilisateur().getCode();
			}

		}

	}

	/**
	 * Getter for utilisateurId.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.utilisateur.UtilisateurDto#utilisateurId utilisateurId}.
	 */
	@Override
	public long getUtilisateurId() {
		return this.utilisateurId;
	}

	/**
	 * Getter for utilisateurEmail.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.utilisateur.UtilisateurDto#utilisateurEmail utilisateurEmail}.
	 */
	@Override
	public String getUtilisateurEmail() {
		return this.utilisateurEmail;
	}

	/**
	 * Getter for utilisateurTypeUtilisateurCode.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.utilisateur.UtilisateurDto#utilisateurTypeUtilisateurCode utilisateurTypeUtilisateurCode}.
	 */
	@Override
	public TypeUtilisateur.Values getUtilisateurTypeUtilisateurCode() {
		return this.utilisateurTypeUtilisateurCode;
	}

	/**
	 * Getter for utilisateurParent.
	 *
	 * @return value of {@link topmodel.exemple.name.dao.dtos.utilisateur.UtilisateurDto#utilisateurParent utilisateurParent}.
	 */
	@Override
	public UtilisateurDto getUtilisateurParent() {
		return this.utilisateurParent;
	}

	/**
	 * Set the value of {@link topmodel.exemple.name.dao.dtos.utilisateur.UtilisateurDto#utilisateurId utilisateurId}.
	 * @param utilisateurId value to set
	 */
	public void setUtilisateurId(long utilisateurId) {
		this.utilisateurId = utilisateurId;
	}

	/**
	 * Set the value of {@link topmodel.exemple.name.dao.dtos.utilisateur.UtilisateurDto#utilisateurEmail utilisateurEmail}.
	 * @param utilisateurEmail value to set
	 */
	public void setUtilisateurEmail(String utilisateurEmail) {
		this.utilisateurEmail = utilisateurEmail;
	}

	/**
	 * Set the value of {@link topmodel.exemple.name.dao.dtos.utilisateur.UtilisateurDto#utilisateurTypeUtilisateurCode utilisateurTypeUtilisateurCode}.
	 * @param utilisateurTypeUtilisateurCode value to set
	 */
	public void setUtilisateurTypeUtilisateurCode(TypeUtilisateur.Values utilisateurTypeUtilisateurCode) {
		this.utilisateurTypeUtilisateurCode = utilisateurTypeUtilisateurCode;
	}

	/**
	 * Set the value of {@link topmodel.exemple.name.dao.dtos.utilisateur.UtilisateurDto#utilisateurParent utilisateurParent}.
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

		dest.setId(this.getUtilisateurId());
		dest.setEmail(this.getUtilisateurEmail());

		return dest;
	}
}
